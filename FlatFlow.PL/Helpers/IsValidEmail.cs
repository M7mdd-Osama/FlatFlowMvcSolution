namespace FlatFlow.PL.Helpers
{
    public static class IsValidEmail
    {
        public static bool IsValidEmailFunction(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }


    }
}
