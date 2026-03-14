using Microsoft.Win32;

namespace Web.API.Helpers
{
    public static class EmailTemplates
    {
        public static string WelcomeEmaiil(string username) =>
            $@"
                <div style='background-color: #f4f7f9; padding: 40px 0; font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; width: 100%;'>
                    <table align='center' border='0' cellpadding='0' cellspacing='0' width='600' style='background-color: #ffffff; border-radius: 12px; box-shadow: 0 4px 12px rgba(0,0,0,0.1); overflow: hidden; border-collapse: collapse;'>
                        <tr>
                            <td align='center' style='background-color: #2c3e50; padding: 30px 0;'>
                                <h1 style='color: #ffffff; margin: 0; font-size: 28px; letter-spacing: 1px;'>StockApp</h1>
                            </td>
                        </tr>
                        <tr>
                            <td style='padding: 40px 30px;'>
                                <h2 style='color: #333333; margin-top: 0;'>Вітаємо, {username}! 👋</h2>
                                <p style='color: #555555; font-size: 16px; line-height: 1.6;'>
                                    Ми дуже раді, що ви приєдналися до <strong>StockApp</strong>. Тепер у вас є потужний інструмент для відстеження акцій та керування своїм портфелем у реальному часі.
                                </p>
                                <div style='text-align: center; margin: 35px 0;'>
                                    <a href='http://localhost:5104/swagger/index.html' style='background-color: #3498db; color: #ffffff; padding: 14px 30px; text-decoration: none; border-radius: 6px; font-weight: bold; font-size: 16px; display: inline-block;'>Головна сторінка</a>
                                </div>
                                <p style='color: #777777; font-size: 14px; border-top: 1px solid #eeeeee; padding-top: 20px;'>
                                    Ваш акаунт було успішно створено. Якщо ви не реєструвалися у нас, просто проігноруйте цей лист.
                                </p>
                            </td>
                        </tr>
                        <tr>
                            <td align='center' style='background-color: #f8f9fa; padding: 20px; color: #999999; font-size: 12px;'>
                                &copy; 2026 StockApp Inc. | Тернопіль, Україна
                            </td>
                        </tr>
                    </table>
                </div>";

    }
}
