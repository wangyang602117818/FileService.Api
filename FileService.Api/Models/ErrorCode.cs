namespace FileService.Api.Models
{
    public enum ErrorCode
    {
        success = 0,

        redirect = 1,
        invalid_params = 105,
        params_valid_fault = 106,
        file_type_blocked = 108,

        record_not_exist = 200,
        task_not_completed = 204,
        file_type_not_match = 206,

        unauthorized = 401,

        invalid_token = 402,
        forbidden = 403,
        invalid_username_or_password = 203,

        invalid_code = 530,
        invalid_authcode = 531,
        server_exception = -1000
    }
}
