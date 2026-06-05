namespace BeneficiosPlataforma.Domain.Common;

public static class MdmStatus
{
    public const string Draft = "DRAFT";
    public const string Golden = "GOLDEN";
    public const string Merged = "MERGED";
    public const string Deprecated = "DEPRECATED";

    public static bool IsValid(string status)
    {
        return status is Draft or Golden or Merged or Deprecated;
    }
}
