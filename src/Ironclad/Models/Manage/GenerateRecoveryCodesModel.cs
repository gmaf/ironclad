// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Models
{
    public class GenerateRecoveryCodesModel
    {
#pragma warning disable CA1819 // JUSTIFICATION (Cameron):  Properties should not return arrays: DTO.
        public string[] RecoveryCodes { get; set; }
    }
}
