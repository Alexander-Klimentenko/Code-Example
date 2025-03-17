using System;

namespace Gateway.Shared.Models.PnC.Settings
{
    public class ExternalCarrierIdOptions
    {
        public ExternalCarrierIdOptions(
            bool useExternalCarrierId,
            string updatedBy,
            DateTime updatedUtc)
        {
            UseExternalCarrierId = useExternalCarrierId;
            UpdatedBy = updatedBy;
            UpdatedUtc = updatedUtc;
        }

        public bool UseExternalCarrierId { get; }
        public string UpdatedBy { get; }
        public DateTime UpdatedUtc { get; }
    }

    public class ExternalCarrierIdOptionsUpdateRequest
    {
        public ExternalCarrierIdOptionsUpdateRequest(
            bool useExternalCarrierId)
        {
            UseExternalCarrierId = useExternalCarrierId;
        }

        public bool UseExternalCarrierId { get; }
    }
}
