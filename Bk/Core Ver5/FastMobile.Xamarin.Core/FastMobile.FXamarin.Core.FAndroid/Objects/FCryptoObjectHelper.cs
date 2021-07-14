using Android.App;
using Android.Security.Keystore;
using AndroidX.Core.Hardware.Fingerprint;
using Java.Security;
using Javax.Crypto;
using System;

namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FCryptoObjectHelper
    {
        private static readonly string KEY_NAME = Application.Context.PackageName;
        private static readonly string KEYSTORE_NAME = "AndroidKeyStore";

        private static readonly string KEY_ALGORITHM = KeyProperties.KeyAlgorithmAes;
        private static readonly string BLOCK_MODE = KeyProperties.BlockModeCbc;
        private static readonly string ENCRYPTION_PADDING = KeyProperties.EncryptionPaddingPkcs7;
        private static readonly string TRANSFORMATION = KEY_ALGORITHM + "/" + BLOCK_MODE + "/" + ENCRYPTION_PADDING;
        private readonly KeyStore keystore;

        public FCryptoObjectHelper()
        {
            keystore = KeyStore.GetInstance(KEYSTORE_NAME);
            keystore.Load(null);
        }

        [Obsolete]
        public FingerprintManagerCompat.CryptoObject BuildCryptoObject()
        {
            return new FingerprintManagerCompat.CryptoObject(CreateCipher());
        }

        private Cipher CreateCipher(bool retry = true)
        {
            var cipher = Cipher.GetInstance(TRANSFORMATION);
            try
            {
                cipher.Init(CipherMode.EncryptMode, GetKey());
            }
            catch (KeyPermanentlyInvalidatedException e)
            {
                keystore.DeleteEntry(KEY_NAME);
                if (retry) CreateCipher(false);
                else throw new Exception("Could not create the cipher for fingerprint authentication.", e);
            }
            return cipher;
        }

        private IKey GetKey()
        {
            if (!keystore.IsKeyEntry(KEY_NAME))
                CreateKey();
            return keystore.GetKey(KEY_NAME, null);
        }

        private void CreateKey()
        {
            var keyGen = KeyGenerator.GetInstance(KeyProperties.KeyAlgorithmAes, KEYSTORE_NAME);
            var keyGenSpec = new KeyGenParameterSpec.Builder(KEY_NAME, KeyStorePurpose.Encrypt | KeyStorePurpose.Decrypt)
                    .SetBlockModes(BLOCK_MODE)
                    .SetEncryptionPaddings(ENCRYPTION_PADDING)
                    .SetUserAuthenticationRequired(true)
                    .Build();
            keyGen.Init(keyGenSpec);
            keyGen.GenerateKey();
        }
    }
}