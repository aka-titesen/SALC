namespace SALC.BLL
{
    public interface IPasswordHasher
    {
        bool Verify(string plainText, string hashed);
        string Hash(string plainText);
    }
}
