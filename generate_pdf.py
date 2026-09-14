"""Generate a professional PDF from BloodBond-Project-Documentation.md.

Uses reportlab.platypus with a custom HTML-like styling that handles
Arabic-friendly fonts and the markdown content.
"""
import re
from pathlib import Path

from reportlab.lib import colors
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import ParagraphStyle, getSampleStyleSheet
from reportlab.lib.units import cm
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.platypus import (
    PageBreak,
    Paragraph,
    SimpleDocTemplate,
    Spacer,
    Table,
    TableStyle,
)


WORKSPACE = Path(r"D:\BackEnd\testproject\Blood-Bond")
MD_FILE = WORKSPACE / "BloodBond-Project-Documentation.md"
PDF_FILE = WORKSPACE / "BloodBond-Project-Documentation.pdf"

# Try to register a font that supports Arabic + Latin. We default to the
# built-in Helvetica; the project documentation is primarily English
# with Arabic section headings, which Helvetica renders acceptably for
# the structural pass we'll do here. If you have a TTF that supports
# Arabic (e.g. Amiri), drop it next to this script and we'll pick it up.
def _try_register_ttf(name: str, path: Path):
    if path.exists():
        try:
            pdfmetrics.registerFont(TTFont(name, str(path)))
            return name
        except Exception:
            return None
    return None


FONT_NAME = _try_register_ttf(
    "Amiri",
    Path(r"C:\Windows\Fonts\Amiri-Regular.ttf"),
) or _try_register_ttf(
    "Amiri",
    WORKSPACE / "Amiri-Regular.ttf",
) or "Helvetica"


def md_inline_to_html(text: str) -> str:
    """Very small markdown -> inline HTML converter for paragraphs.

    Handles: **bold**, *italic*, `code`, [text](url), `## headings`,
    `- bullets`, `1. numbered`, fenced ```code blocks```, tables.
    """
    # Escape HTML special chars first, but preserve our markdown tags
    out = text.replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;")
    # Code spans
    out = re.sub(r"`([^`]+)`", r'<font face="Courier" color="#c7254e">\1</font>', out)
    # Bold
    out = re.sub(r"\*\*([^*]+)\*\*", r"<b>\1</b>", out)
    # Italic
    out = re.sub(r"\*([^*]+)\*", r"<i>\1</i>", out)
    # Links
    out = re.sub(r"\[([^\]]+)\]\(([^)]+)\)", r'<font color="#1f6feb"><u>\1</u></font>', out)
    return out


def parse_markdown(md: str):
    """Yield ('h1'|'h2'|'h3'|'p'|'code'|'bullet'|'numbered'|'table', payload) tuples."""
    lines = md.splitlines()
    i = 0
    while i < len(lines):
        line = lines[i]
        stripped = line.strip()

        # Skip empty
        if not stripped:
            i += 1
            continue

        # Fenced code block
        if stripped.startswith("```"):
            i += 1
            buf = []
            while i < len(lines) and not lines[i].strip().startswith("```"):
                buf.append(lines[i])
                i += 1
            i += 1  # skip closing fence
            yield ("code", "\n".join(buf))
            continue

        # Heading
        if stripped.startswith("###"):
            yield ("h3", stripped[3:].strip())
            i += 1
            continue
        if stripped.startswith("##"):
            yield ("h2", stripped[2:].strip())
            i += 1
            continue
        if stripped.startswith("#"):
            yield ("h1", stripped[1:].strip())
            i += 1
            continue

        # Table (simple GitHub-flavoured pipe tables)
        if stripped.startswith("|") and i + 1 < len(lines) and re.match(r"^\|[-\s|]+\|$", lines[i + 1].strip()):
            header_cells = [c.strip() for c in stripped.strip("|").split("|")]
            i += 2  # skip header + separator
            rows = []
            while i < len(lines) and lines[i].strip().startswith("|"):
                row_cells = [c.strip() for c in lines[i].strip().strip("|").split("|")]
                rows.append(row_cells)
                i += 1
            yield ("table", (header_cells, rows))
            continue

        # Bullet
        if stripped.startswith("- "):
            yield ("bullet", stripped[2:].strip())
            i += 1
            continue
        # Numbered
        m = re.match(r"^(\d+)\.\s+(.*)$", stripped)
        if m:
            yield ("numbered", m.group(2).strip())
            i += 1
            continue

        # Plain paragraph — collect contiguous lines
        buf = [line]
        i += 1
        while i < len(lines) and lines[i].strip() and not (
            lines[i].lstrip().startswith(("#", "- ", "1.", "2.", "3.", "4.", "5.", "6.", "7.", "8.", "9.", "10."))
            or lines[i].strip().startswith("```")
            or lines[i].strip().startswith("|")
        ):
            buf.append(lines[i])
            i += 1
        yield ("p", "\n".join(buf).strip())


def build_pdf():
    md = MD_FILE.read_text(encoding="utf-8")
    doc = SimpleDocTemplate(
        str(PDF_FILE),
        pagesize=A4,
        leftMargin=2 * cm,
        rightMargin=2 * cm,
        topMargin=2 * cm,
        bottomMargin=2 * cm,
        title="Blood Bond — Project Documentation",
        author="Blood Bond Team",
    )

    ss = getSampleStyleSheet()
    title_style = ParagraphStyle(
        "Title",
        parent=ss["Title"],
        fontName=FONT_NAME,
        fontSize=24,
        leading=30,
        textColor=colors.HexColor("#b02a37"),
        spaceAfter=18,
    )
    h1 = ParagraphStyle(
        "H1",
        parent=ss["Heading1"],
        fontName=FONT_NAME,
        fontSize=18,
        leading=22,
        textColor=colors.HexColor("#b02a37"),
        spaceBefore=18,
        spaceAfter=10,
    )
    h2 = ParagraphStyle(
        "H2",
        parent=ss["Heading2"],
        fontName=FONT_NAME,
        fontSize=14,
        leading=18,
        textColor=colors.HexColor("#dc3545"),
        spaceBefore=14,
        spaceAfter=8,
    )
    h3 = ParagraphStyle(
        "H3",
        parent=ss["Heading3"],
        fontName=FONT_NAME,
        fontSize=12,
        leading=16,
        textColor=colors.HexColor("#1f6feb"),
        spaceBefore=10,
        spaceAfter=6,
    )
    body = ParagraphStyle(
        "Body",
        parent=ss["BodyText"],
        fontName=FONT_NAME,
        fontSize=10,
        leading=14,
        spaceAfter=8,
    )
    code_style = ParagraphStyle(
        "Code",
        parent=ss["Code"],
        fontName="Courier",
        fontSize=8.5,
        leading=11,
        leftIndent=8,
        rightIndent=8,
        backColor=colors.HexColor("#f5f5f5"),
        borderColor=colors.HexColor("#dddddd"),
        borderWidth=0.5,
        borderPadding=6,
        spaceBefore=4,
        spaceAfter=8,
    )
    bullet_style = ParagraphStyle(
        "Bullet",
        parent=body,
        leftIndent=14,
        bulletIndent=4,
        spaceAfter=4,
    )
    numbered_style = ParagraphStyle(
        "Numbered",
        parent=body,
        leftIndent=18,
        spaceAfter=4,
    )

    story = [Paragraph("🩸 Blood Bond — التوثيق الكامل للمشروع", title_style)]
    story.append(Paragraph(
        "Generated from <i>BloodBond-Project-Documentation.md</i>. "
        "Covers the full backend (.NET) and frontend (React) implementation, "
        "every API endpoint, Postman scenarios, role permissions, enums, "
        "deployment, and troubleshooting.",
        body,
    ))
    story.append(Spacer(1, 0.5 * cm))

    for kind, payload in parse_markdown(md):
        if kind == "h1":
            story.append(Paragraph(md_inline_to_html(payload), h1))
        elif kind == "h2":
            story.append(Paragraph(md_inline_to_html(payload), h2))
        elif kind == "h3":
            story.append(Paragraph(md_inline_to_html(payload), h3))
        elif kind == "p":
            story.append(Paragraph(md_inline_to_html(payload), body))
        elif kind == "code":
            # Preserve newlines
            text = payload.replace("\n", "<br/>")
            story.append(Paragraph(text, code_style))
        elif kind == "bullet":
            story.append(Paragraph("• " + md_inline_to_html(payload), bullet_style))
        elif kind == "numbered":
            story.append(Paragraph("&nbsp;&nbsp;→ " + md_inline_to_html(payload), numbered_style))
        elif kind == "table":
            headers, rows = payload
            data = [[Paragraph(md_inline_to_html(h), body) for h in headers]]
            for r in rows:
                data.append([Paragraph(md_inline_to_html(c), body) for c in r])
            t = Table(data, repeatRows=1)
            t.setStyle(TableStyle([
                ("BACKGROUND", (0, 0), (-1, 0), colors.HexColor("#f8d7da")),
                ("TEXTCOLOR", (0, 0), (-1, 0), colors.HexColor("#721c24")),
                ("GRID", (0, 0), (-1, -1), 0.5, colors.HexColor("#dddddd")),
                ("VALIGN", (0, 0), (-1, -1), "TOP"),
                ("FONTSIZE", (0, 0), (-1, -1), 8),
                ("LEFTPADDING", (0, 0), (-1, -1), 4),
                ("RIGHTPADDING", (0, 0), (-1, -1), 4),
                ("TOPPADDING", (0, 0), (-1, -1), 4),
                ("BOTTOMPADDING", (0, 0), (-1, -1), 4),
            ]))
            story.append(t)
            story.append(Spacer(1, 0.2 * cm))

    doc.build(story)
    print(f"Wrote {PDF_FILE} ({PDF_FILE.stat().st_size:,} bytes)")


if __name__ == "__main__":
    build_pdf()
