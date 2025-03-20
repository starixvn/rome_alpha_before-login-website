using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Alpha.Before.Criteria;

public class InsertPersonalMessageCriteria : NonQueryCommand
{
    public InsertPersonalMessageCriteria(
    string contentEN,
    string contentCH,
    string contentCS,
    string contentTH,
    string contentVI,
    string contentJP,
    string contentKO)
    {
        ContentEN = contentEN;
        ContentCH = contentCH;
        ContentCS = contentCS;
        ContentTH = contentTH;
        ContentVN = contentVI;
        ContentJP = contentJP;
        ContentKO = contentKO;
    }

    private const int PersonalMessageType = 1;

    [SpParam(SqlParamName = "content_en")]
    public string ContentEN { get; }

    [SpParam(SqlParamName = "content_ch")]
    public string ContentCH { get; }

    [SpParam(SqlParamName = "content_cs")]
    public string ContentCS { get; }

    [SpParam(SqlParamName = "content_jp")]
    public string ContentJP { get; }

    [SpParam(SqlParamName = "content_it")]
    public string ContentIT => string.Empty;

    [SpParam(SqlParamName = "content_th")]
    public string ContentTH { get; }

    [SpParam(SqlParamName = "content_ko")]
    public string ContentKO { get; }

    [SpParam(SqlParamName = "content_es")]
    public string ContentES => string.Empty;

    [SpParam(SqlParamName = "content_vn")]
    public string ContentVN { get; }

    [SpParam(SqlParamName = "msgtype")]
    public int MessageType { get; } = PersonalMessageType;

    public string CreatorName { get; } = string.Empty;

    public bool IsBatch { get; } = true;

    public int? MessageIdOutput { get; set; }

    public override string GetSettingKey() => "Age_Msg_SetMessage";

    public override bool IsValid() => true;
}
