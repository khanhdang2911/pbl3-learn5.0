using BCrypt.Net;

public class HashPasswordByBC
{
    public string HashPassword(string password)
    {
        // Tạo salt và hash mật khẩu
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        // Lưu hashedPassword vào cơ sở dữ liệu
        return hashedPassword;
    }
    public bool VerifyPassword(string password, string hashedPassword)
    {
        // Kiểm tra mật khẩu nhập vào với hashedPassword từ cơ sở dữ liệu
        bool isMatch = BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        return isMatch;
    }


}