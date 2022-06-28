namespace ActionCode.Persistence
{
    public static class CryptographerFactory
    {
        public static ICryptographer Create(CryptographerType type, string key)
        {
            return type switch
            {
                CryptographerType.None => null,
                CryptographerType.AES => new AESCryptographer(key),
                _ => null,
            };
        }
    }
}