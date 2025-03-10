﻿#if NET35 || NET40 || NET45 || NET451
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

namespace System.Security.Cryptography
{
    /// <summary>
    /// Specifies the padding mode and parameters to use with RSA encryption or decryption operations.
    /// </summary>
    public sealed class RSAEncryptionPadding : IEquatable<RSAEncryptionPadding>
    {
        private static readonly RSAEncryptionPadding s_pkcs1 = new RSAEncryptionPadding(RSAEncryptionPaddingMode.Pkcs1, default(HashAlgorithmName));
        private static readonly RSAEncryptionPadding s_oaepSHA1 = CreateOaep(HashAlgorithmName.SHA1);
        private static readonly RSAEncryptionPadding s_oaepSHA256 = CreateOaep(HashAlgorithmName.SHA256);
        private static readonly RSAEncryptionPadding s_oaepSHA384 = CreateOaep(HashAlgorithmName.SHA384);
        private static readonly RSAEncryptionPadding s_oaepSHA512 = CreateOaep(HashAlgorithmName.SHA512);

        /// <summary>
        /// <see cref="RSAEncryptionPaddingMode.Pkcs1"/> mode.
        /// </summary>
        public static RSAEncryptionPadding Pkcs1 { get { return s_pkcs1; } }

        /// <summary>
        /// <see cref="RSAEncryptionPaddingMode.Oaep"/> mode with SHA1 hash algorithm.
        /// </summary>
        public static RSAEncryptionPadding OaepSHA1 { get { return s_oaepSHA1; } }

        /// <summary>
        /// <see cref="RSAEncryptionPaddingMode.Oaep"/> mode with SHA256 hash algorithm.
        /// </summary>
        public static RSAEncryptionPadding OaepSHA256 { get { return s_oaepSHA256; } }

        /// <summary>
        /// <see cref="RSAEncryptionPaddingMode.Oaep"/> mode with SHA384 hash algorithm.
        /// </summary>
        public static RSAEncryptionPadding OaepSHA384 { get { return s_oaepSHA384; } }

        /// <summary>
        /// <see cref="RSAEncryptionPaddingMode.Oaep"/> mode with SHA512 hash algorithm.
        /// </summary>
        public static RSAEncryptionPadding OaepSHA512 { get { return s_oaepSHA512; } }

        private readonly RSAEncryptionPaddingMode _mode;
        private readonly HashAlgorithmName _oaepHashAlgorithm;

        private RSAEncryptionPadding(RSAEncryptionPaddingMode mode, HashAlgorithmName oaepHashAlgorithm)
        {
            _mode = mode;
            _oaepHashAlgorithm = oaepHashAlgorithm;
        }

        /// <summary>
        /// Creates a new instance representing <see cref="RSAEncryptionPaddingMode.Oaep"/>
        /// with the given hash algorithm.
        /// </summary>
        public static RSAEncryptionPadding CreateOaep(HashAlgorithmName hashAlgorithm)
        {
            if (string.IsNullOrEmpty(hashAlgorithm.Name))
            {
                throw new ArgumentException(SR.Cryptography_HashAlgorithmNameNullOrEmpty, nameof(hashAlgorithm));
            }

            return new RSAEncryptionPadding(RSAEncryptionPaddingMode.Oaep, hashAlgorithm);
        }

        /// <summary>
        /// Gets the padding mode to use.
        /// </summary>
        public RSAEncryptionPaddingMode Mode
        {
            get { return _mode; }
        }

        /// <summary>
        /// Gets the padding mode to use in conjunction with <see cref="RSAEncryptionPaddingMode.Oaep"/>.
        /// </summary>
        /// <remarks>
        /// If <see cref="Mode"/> is not <see cref="RSAEncryptionPaddingMode.Oaep"/>, then <see cref="HashAlgorithmName.Name" /> will be null.
        /// </remarks>
        public HashAlgorithmName OaepHashAlgorithm
        {
            get { return _oaepHashAlgorithm; }
        }

        public override int GetHashCode()
        {
            return CombineHashCodes(_mode.GetHashCode(), _oaepHashAlgorithm.GetHashCode());
        }

        // Same as non-public Tuple.CombineHashCodes
        private static int CombineHashCodes(int h1, int h2)
        {
            return (((h1 << 5) + h1) ^ h2);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as RSAEncryptionPadding);
        }

        public bool Equals(RSAEncryptionPadding other)
        {
            return other is not null
                && _mode == other._mode
                && _oaepHashAlgorithm == other._oaepHashAlgorithm;
        }

        public static bool operator ==(RSAEncryptionPadding left, RSAEncryptionPadding right)
        {
            if (left is null)
            {
                return right is null;
            }

            return left.Equals(right);
        }

        public static bool operator !=(RSAEncryptionPadding left, RSAEncryptionPadding right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return _mode.ToString() + _oaepHashAlgorithm.Name;
        }
    }
}
#endif
