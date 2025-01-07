using Fanex.Alpha.Before.Configurations;
using Fanex.Data.Repository;

namespace Fanex.Alpha.Before.Criteria
{
    public class UpdateProfilePictureCommand : NonQueryCommand
    {
        public UpdateProfilePictureCommand(int custId, string ip, bool isDefault = true)
        {
            CustId = custId;
            IP = ip;
            IsDefault = isDefault;
        }

        public int CustId { get; }

        public int Mode => AppSetting.Is789ySite ? 2 : 1;

        public bool IsDefault { get; }

        public bool IsMainServer => true;

        public string IP { get; }

        public byte[] Image { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey() => "Age_CustomerUpdProfilePicture";

        public override bool IsValid() => CustId > 0;
    }
}