namespace Fanex.Alpha.Before.Criteria
{
    public class VerifyDownlineLoginCommand
    {
        public string Username { get; set; }

        public string UserPwd { get; set; }

        public int CustId { get; set; }

        public int SubAccId { get; set; }

        public int ServerId { get; set; }

        public DateTime LoginTime { get; set; }

        public string Language { get; set; }

        public bool IsNickname { get; set; }

        public string Nickname { get; set; }

        public int ErrCode { get; set; }

        public int ErrMsg { get; set; }

        public DateTime LastChangedPwdDate { get; set; }

        public int SuperId { get; set; }

        public string Site { get; set; }

        public byte RoleId { get; set; }

        public string SubUsername { get; set; }

        public bool IsKickOut { get; set; }


    }
}