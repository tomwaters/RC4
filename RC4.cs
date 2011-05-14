using System;
using System.Text;
using System.Runtime.Serialization;

namespace RandomWorks
{
    [Serializable]
    public class KeyNotSetException : Exception
    {
        public KeyNotSetException() : base(){}
        public KeyNotSetException(string message) : base(message) { }
        public KeyNotSetException(string message, Exception innerException) : base(message, innerException) { }  
    }

    public class RC4
    {
        private int[] S = null;

        //key-scheduling algorithm
        public void SetKey(string key)
        {
            S = new int[256];
            int keyLength = key.Length;

            if (keyLength < 1)
                throw new KeyNotSetException("The key has not been set");

            for (int i = 0; i <= 255; i++)
                S[i] = (byte)i;

            int j = 0;
            for (int i = 0; i <= 255; i++)
            {
                j = (j + S[i] + key[i % keyLength]) % 256;
                int iTmp = S[i];
                S[i] = S[j];
                S[j] = iTmp;
            }
        }

        //pseudo-random generation algorithm
        private int[] PRGA(int length)
        {
            int i = 0;
            int j = 0;
            int[] keystream = new int[length];

            for (int a = 0; a < length; a++)
            {
                i = (i + 1) % 256;
                j = (j + S[i]) % 256;

                int iTmp = S[i];
                S[i] = S[j];
                S[j] = iTmp;
                keystream[a] = S[(S[i] + S[j]) % 256];
            }

            return keystream;
        }

        //encrypt or decrypt the supplied int array
        private int[] Process(int[] input)
        {
            int length = input.Length;
            int[] keyStream = PRGA(length);
            int[] encrypted = new int[length];

            for (int i = 0; i < length; i++)
            {
                encrypted[i] = input[i] ^ keyStream[i];
            }

            return encrypted;
        }

        public int[] GetKeyStream(int length)
        {
            return PRGA(length);
        }

        public string Decrypt(int[] ciphertext)
        {
            int[] plaintext = Process(ciphertext);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < plaintext.Length; i++)
                sb.Append((char)plaintext[i]);
            
            return sb.ToString();
        }

        public int[] Encrypt(string plaintext)
        {
            int[] input = new int[plaintext.Length];
            for(int i=0; i<plaintext.Length; i++)
                input[i] = (int)plaintext[i];
            
            return Process(input);
        }
    }
}
