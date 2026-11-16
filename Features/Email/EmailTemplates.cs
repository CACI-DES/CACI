using System;
using CACI_Life.Features.Discovery;

namespace CACI_Life.Features.Email;

public static class EmailTemplates
{
    public static string GetAdminNotificationBody(string title, string detailsHtml)
    {
        return $@"
            <div style='font-family: Arial, sans-serif; color: #333; max-width: 600px;'>
                <h2 style='color: #174477; border-bottom: 2px solid #C8102E; padding-bottom: 10px;'>{title}</h2>
                <div style='background: #f9f9f9; padding: 15px; border-radius: 5px; margin-top: 20px;'>
                    {detailsHtml}
                </div>
                <p style='color: #666; font-size: 12px; margin-top: 30px;'>
                    Sent from <strong>CACI Website</strong> (caci.life)
                </p>
            </div>";
    }

    public static string GetUserConfirmationBody(string name, string requestType)
    {
        return $@"
            <div style='font-family: Arial, sans-serif; color: #333; max-width: 600px;'>
                <div style='background: #174477; padding: 20px; text-align: center; border-radius: 6px 6px 0 0;'>
                    <h1 style='color: white; margin: 0; font-size: 24px;'>Thank You!</h1>
                </div>
                <div style='padding: 25px; border: 1px solid #ddd; border-top: none; border-radius: 0 0 6px 6px;'>
                    <p>Dear <strong>{name}</strong>,</p>
                    <p>We have received your message regarding <strong>{requestType}</strong>.</p>
                    <p>A member of our team will review your message and follow up with you shortly.</p>
                    <br>
                    <p>Blessings,</p>
                    <p><strong>Covenant Apostolic Congregations International</strong></p>
                    <a href='https://caci.life' style='color: #174477; font-weight: bold;'>www.caci.life</a>
                </div>
            </div>";
    }

    public static string GetSpiritualGiftsResultsBody(SpiritualGiftsEntry entry)
    {
        return $@"
        <div style='background-color: #f4f4f4; padding: 20px; font-family: sans-serif;'>
            <div style='background-color: white; padding: 30px; border-radius: 6px; max-width: 600px; margin: 0 auto; border-top: 5px solid #174477;'>
                <h2 style='color: #174477; text-align: center;'>Spiritual Gifts Discovery Results</h2>
                <p>Hello {entry.FirstName},</p>
                <p>Thank you for completing your Spiritual Gifts Discovery for {entry.SubmissionDate:yyyy}. Below are the scores you entered:</p>
                
                <table style='width: 100%; border-collapse: collapse; margin: 20px 0;'>
                    <tr style='background-color: #174477; color: white;'>
                        <th style='padding: 10px; text-align: left;'>Spiritual Gift</th>
                        <th style='padding: 10px; text-align: center;'>Score</th>
                    </tr>
                    <tr><td style='padding: 8px; border-bottom: 1px solid #eee;'>Administration</td><td style='padding: 8px; border-bottom: 1px solid #eee; text-align: center;'>{entry.Administration}</td></tr>
                    <tr><td style='padding: 8px; border-bottom: 1px solid #eee;'>Apostleship</td><td style='padding: 8px; border-bottom: 1px solid #eee; text-align: center;'>{entry.Apostleship}</td></tr>
                    <tr><td style='padding: 8px; border-bottom: 1px solid #eee;'>Discernment</td><td style='padding: 8px; border-bottom: 1px solid #eee; text-align: center;'>{entry.Discernment}</td></tr>
                    <tr><td style='padding: 8px; border-bottom: 1px solid #eee;'>Evangelism</td><td style='padding: 8px; border-bottom: 1px solid #eee; text-align: center;'>{entry.Evangelism}</td></tr>
                    <tr><td style='padding: 8px; border-bottom: 1px solid #eee;'>Exhortation</td><td style='padding: 8px; border-bottom: 1px solid #eee; text-align: center;'>{entry.Exhortation}</td></tr>
                    <tr><td style='padding: 8px; border-bottom: 1px solid #eee;'>Faith</td><td style='padding: 8px; border-bottom: 1px solid #eee; text-align: center;'>{entry.Faith}</td></tr>
                    <tr><td style='padding: 8px; border-bottom: 1px solid #eee;'>Giving</td><td style='padding: 8px; border-bottom: 1px solid #eee; text-align: center;'>{entry.Giving}</td></tr>
                    <tr><td style='padding: 8px; border-bottom: 1px solid #eee;'>Knowledge</td><td style='padding: 8px; border-bottom: 1px solid #eee; text-align: center;'>{entry.Knowledge}</td></tr>
                    <tr><td style='padding: 8px; border-bottom: 1px solid #eee;'>Leadership</td><td style='padding: 8px; border-bottom: 1px solid #eee; text-align: center;'>{entry.Leadership}</td></tr>
                    <tr><td style='padding: 8px; border-bottom: 1px solid #eee;'>Mercy</td><td style='padding: 8px; border-bottom: 1px solid #eee; text-align: center;'>{entry.Mercy}</td></tr>
                    <tr><td style='padding: 8px; border-bottom: 1px solid #eee;'>Pastoring / Shepherding</td><td style='padding: 8px; border-bottom: 1px solid #eee; text-align: center;'>{entry.Pastoring}</td></tr>
                    <tr><td style='padding: 8px; border-bottom: 1px solid #eee;'>Prophecy</td><td style='padding: 8px; border-bottom: 1px solid #eee; text-align: center;'>{entry.Prophecy}</td></tr>
                    <tr><td style='padding: 8px; border-bottom: 1px solid #eee;'>Serving / Helps</td><td style='padding: 8px; border-bottom: 1px solid #eee; text-align: center;'>{entry.Serving}</td></tr>
                    <tr><td style='padding: 8px; border-bottom: 1px solid #eee;'>Teaching</td><td style='padding: 8px; border-bottom: 1px solid #eee; text-align: center;'>{entry.Teaching}</td></tr>
                    <tr><td style='padding: 8px; border-bottom: 1px solid #eee;'>Wisdom</td><td style='padding: 8px; border-bottom: 1px solid #eee; text-align: center;'>{entry.Wisdom}</td></tr>
                </table>

                <p>By monitoring these scores annually, you can see how God is working in your life and developing your gifts.</p>
                <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>
                <p style='color: #555; text-align: center; font-size: 0.9em;'>Covenant Apostolic Congregations International</p>
            </div>
        </div>";
    }
}
