using System.Diagnostics;
using TrackPoint.Data;
using TrackPoint.Models;

namespace TrackPoint.Services
{
    public class TransferLogService
    {
        private readonly ApplicationDbContext _context;

        public TransferLogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TransferLog> TransferLogAsync(int assetId, string borrowerId, string assetStatus, Enum eventType, DateTime transferDate)
        {
            var asset = _context.Asset.FirstOrDefault(a => a.AssetId == assetId);
            var tLogEntry = new TransferLog
            {
                AssetId = assetId,
                BorrowerId = borrowerId,
                OldStatus = asset.AssetStatus,
                NewStatus = assetStatus, // TODO: Should be an enum
                eventType = (Enums.eventType)eventType, // TODO: Is this cast really necessary?
                TransferDate = transferDate
            };
            _context.TransferLog.Add(tLogEntry); // Stage the changes
            return tLogEntry;
        }
    }
}
