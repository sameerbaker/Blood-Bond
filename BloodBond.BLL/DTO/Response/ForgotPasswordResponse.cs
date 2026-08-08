namespace BloodBond.DAL.DTO.Response
{
    public class ForgotPasswordResponse
    {
        public string Message { get; set; } = string.Empty;
        public string? ResetToken { get; set; } 
    }
}
