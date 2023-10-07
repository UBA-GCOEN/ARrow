namespace Gpm.Common.Multilanguage
{
    public delegate void MultilanguageCallback(MultilanguageResultCode result, string resultMessage);

    public enum MultilanguageResultCode
    {
        SUCCESS,

        ALREADY_LOADED,
        NOT_LOADED,

        FILE_NOT_FOUND,
        FILE_LOAD_FAILED,
        FILE_PARSING_ERROR,

        LANGUAGE_LIST_EMPTY,
        LANGUAGE_CODE_NOT_FOUND,

        SERVICE_NOT_FOUND,
    }
}