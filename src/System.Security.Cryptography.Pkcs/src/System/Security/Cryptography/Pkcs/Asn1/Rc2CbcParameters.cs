// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
    // https://tools.ietf.org/html/rfc3370#section-5.2 (CMS Algorithms, RC2-CBC) says
    //
    //    The AlgorithmIdentifier parameters field MUST be present, and the
    //    parameters field MUST contain a RC2CBCParameter:
    //
    // RC2CBCParameter ::= SEQUENCE {
    //   rc2ParameterVersion INTEGER,
    //   iv OCTET STRING  }  -- exactly 8 octets
    //
    // It then effectively says "see RFC2268" for the version.
    //
    // https://tools.ietf.org/html/rfc2268#section-6 provides the table (EkbEncoding),
    // and provides a different structure for "RC2-CBCParameter" (with a hyphen in this name).
    //
    // The RFC3370 structure is the second CHOICE option for RC2-CBCParameter (it has no name).
    // Since 3370 says to just use that alternative there's no fallback in this code for handling
    // just an IV which means that an effective key size of 32-bits has been chosen.  Since 40-bit is the
    // smallest supported by .NET that's not really a problem.
    [StructLayout(LayoutKind.Sequential)]
    internal struct Rc2CbcParameters
    {
        private static readonly byte[] s_rc2EkbEncoding =
        {
            0xbd, 0x56, 0xea, 0xf2, 0xa2, 0xf1, 0xac, 0x2a, 0xb0, 0x93, 0xd1, 0x9c, 0x1b, 0x33, 0xfd, 0xd0,
            0x30, 0x04, 0xb6, 0xdc, 0x7d, 0xdf, 0x32, 0x4b, 0xf7, 0xcb, 0x45, 0x9b, 0x31, 0xbb, 0x21, 0x5a,
            0x41, 0x9f, 0xe1, 0xd9, 0x4a, 0x4d, 0x9e, 0xda, 0xa0, 0x68, 0x2c, 0xc3, 0x27, 0x5f, 0x80, 0x36,
            0x3e, 0xee, 0xfb, 0x95, 0x1a, 0xfe, 0xce, 0xa8, 0x34, 0xa9, 0x13, 0xf0, 0xa6, 0x3f, 0xd8, 0x0c,
            0x78, 0x24, 0xaf, 0x23, 0x52, 0xc1, 0x67, 0x17, 0xf5, 0x66, 0x90, 0xe7, 0xe8, 0x07, 0xb8, 0x60,
            0x48, 0xe6, 0x1e, 0x53, 0xf3, 0x92, 0xa4, 0x72, 0x8c, 0x08, 0x15, 0x6e, 0x86, 0x00, 0x84, 0xfa,
            0xf4, 0x7f, 0x8a, 0x42, 0x19, 0xf6, 0xdb, 0xcd, 0x14, 0x8d, 0x50, 0x12, 0xba, 0x3c, 0x06, 0x4e,
            0xec, 0xb3, 0x35, 0x11, 0xa1, 0x88, 0x8e, 0x2b, 0x94, 0x99, 0xb7, 0x71, 0x74, 0xd3, 0xe4, 0xbf,
            0x3a, 0xde, 0x96, 0x0e, 0xbc, 0x0a, 0xed, 0x77, 0xfc, 0x37, 0x6b, 0x03, 0x79, 0x89, 0x62, 0xc6,
            0xd7, 0xc0, 0xd2, 0x7c, 0x6a, 0x8b, 0x22, 0xa3, 0x5b, 0x05, 0x5d, 0x02, 0x75, 0xd5, 0x61, 0xe3,
            0x18, 0x8f, 0x55, 0x51, 0xad, 0x1f, 0x0b, 0x5e, 0x85, 0xe5, 0xc2, 0x57, 0x63, 0xca, 0x3d, 0x6c,
            0xb4, 0xc5, 0xcc, 0x70, 0xb2, 0x91, 0x59, 0x0d, 0x47, 0x20, 0xc8, 0x4f, 0x58, 0xe0, 0x01, 0xe2,
            0x16, 0x38, 0xc4, 0x6f, 0x3b, 0x0f, 0x65, 0x46, 0xbe, 0x7e, 0x2d, 0x7b, 0x82, 0xf9, 0x40, 0xb5,
            0x1d, 0x73, 0xf8, 0xeb, 0x26, 0xc7, 0x87, 0x97, 0x25, 0x54, 0xb1, 0x28, 0xaa, 0x98, 0x9d, 0xa5,
            0x64, 0x6d, 0x7a, 0xd4, 0x10, 0x81, 0x44, 0xef, 0x49, 0xd6, 0xae, 0x2e, 0xdd, 0x76, 0x5c, 0x2f,
            0xa7, 0x1c, 0xc9, 0x09, 0x69, 0x9a, 0x83, 0xcf, 0x29, 0x39, 0xb9, 0xe9, 0x4c, 0xff, 0x43, 0xab,
        };

        internal int Rc2Version;

        [OctetString]
        internal ReadOnlyMemory<byte> Iv;

        internal Rc2CbcParameters(ReadOnlyMemory<byte> iv, int keySize)
        {
            if (keySize > byte.MaxValue)
            {
                Rc2Version = keySize;
            }
            else
            {
                Rc2Version = s_rc2EkbEncoding[keySize];
            }

            Iv = iv;
        }

        internal int GetEffectiveKeyBits()
        {
            if (Rc2Version > byte.MaxValue)
            {
                return Rc2Version;
            }

            return Array.IndexOf(s_rc2EkbEncoding, (byte)Rc2Version);
        }
    }
}
