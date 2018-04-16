// -----------------------------------------------------------------------
// <copyright file="CaptchaImage.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.Utility.CaptchaUtility
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using Fanex.Alpha.Before.Common.Extensions;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CaptchaImage
    {
        private readonly Random _random = new Random();

        public CaptchaImage(string ip, string username)
        {
            Ip = ip;
            Username = username;
            Choice = GetChoice();
        }

        public CaptchaImage(string ip, string username, CaptchaType type)
        {
            Ip = ip;
            Username = username;
            Choice = type;
        }

        public CaptchaType Choice
        {
            get;
            private set;
        }

        public string Ip
        {
            get;
            private set;
        }

        public string Username
        {
            get;
            private set;
        }

        /// <summary>
        /// Get choice.
        /// </summary>
        /// <returns>Return result.</returns>
        public CaptchaType GetChoice()
        {
            CaptchaType result;
            if (string.IsNullOrEmpty(Ip) || string.IsNullOrEmpty(Username))
            {
                result = (CaptchaType)_random.Next(Enum.GetNames(typeof(CaptchaType)).Length);
            }
            else
            {
                CaptchaInfo info = CaptchaLogging.Find(Ip, Username);

                result = info == null
                    ? (CaptchaType)_random.Next(Enum.GetNames(typeof(CaptchaType)).Length)
                    : info.Type;
            }

            return result;
        }

        /// <summary>
        /// Generate captcha image.
        /// </summary>
        /// <param name="code">The captcha code.</param>
        /// <param name="value">The captcha value.</param>
        /// <returns>Return result.</returns>
        public Image Generate(out string code, out string value)
        {
            code = string.Empty;
            value = string.Empty;

            switch (Choice)
            {
                case CaptchaType.Text:
                    code = EncryptExtension.RandomNumberString(4).ToUpperInvariant();
                    value = code;
                    break;

                case CaptchaType.Question:
                    int firstValue = _random.Next(1, 10);
                    int secondValue = _random.Next(1, 10);
                    if (firstValue < secondValue)
                    {
                        int tmp = firstValue;
                        firstValue = secondValue;
                        secondValue = tmp;
                    }

                    int calculator = _random.Next(3);
                    switch (calculator)
                    {
                        case 0: // add
                            code = string.Format("{0}+{1}", firstValue, secondValue);
                            value = (firstValue + secondValue).ToString();
                            break;
                        case 1: // substract
                            code = string.Format("{0}-{1}", firstValue, secondValue);
                            value = (firstValue - secondValue).ToString();
                            break;
                        case 2: // multiple
                            code = string.Format("{0}*{1}", firstValue, secondValue);
                            value = (firstValue * secondValue).ToString();
                            break;
                    }

                    code += "=";
                    break;
            }

            return CreateImage(code);
        }

        /// <summary>
        /// Create captcha image.
        /// </summary>
        /// <param name="text">The captcha value.</param>
        /// <returns>Return result.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private Image CreateImage(string text)
        {
            int width = 66;
            int height = 24;
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            // Create a graphics object for drawing.
            Graphics g = Graphics.FromImage(bitmap);
            g.CompositingQuality = CompositingQuality.HighSpeed;
            g.SmoothingMode = SmoothingMode.HighSpeed;
            Rectangle rect = new Rectangle(0, 0, width, height);

            // Fill in the background.
            using (HatchBrush hatchBrush = new HatchBrush(HatchStyle.Cross, Color.White, Color.WhiteSmoke))
            {
                g.FillRectangle(hatchBrush, rect);
                g.DrawRectangle(Pens.Gray, 0, 0, width - 1, height - 1);
            }

            // Random font name
            var fonts = new[] { "Century Schoolbook", "Lucida Console", "Georgia", "Courier", "Trebuchet MS", "Lucida Console", "Times New Roman" };
            string familyName = fonts[_random.Next(0, fonts.Length - 1)];

            // Set up the text font.
            SizeF size;
            float fontSize = rect.Height + 80;
            Font font;
            do
            {
                fontSize--;
                font = new Font(familyName, fontSize, FontStyle.Bold);
                size = g.MeasureString(text, font);
            }
            while (size.Width > rect.Width);

            // Set up the text format.
            StringFormat format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            // Create a path using the text and warp it randomly.
            GraphicsPath path = new GraphicsPath();
            path.AddString(text, font.FontFamily, (int)font.Style, 21, rect, format);

            using (Matrix matrix = new Matrix())
            {
                float v = 4F;
                PointF[] points =
                {
                                      new PointF(_random.Next(rect.Width) / v, _random.Next(rect.Height) / v),
                                      new PointF(rect.Width - (_random.Next(rect.Width) / v), _random.Next(rect.Height) / v),
                                      new PointF(this._random.Next(rect.Width) / v, rect.Height - (_random.Next(rect.Height) / v)),
                                      new PointF(rect.Width - (_random.Next(rect.Width) / v), rect.Height - (_random.Next(rect.Height) / v))
                                  };
                matrix.Translate(0F, 0F);
                path.Warp(points, rect, matrix, WarpMode.Perspective, 0F);
            }

            // Draw the text.
            using (HatchBrush hatchBrush = new HatchBrush(HatchStyle.LargeConfetti, Color.Gray, Color.Gray))
            {
                g.FillPath(hatchBrush, path);

                // Add some random noise.
                int m = Math.Max(rect.Width + 100, rect.Height + 100);
                for (int i = 0; i < (int)(rect.Width * rect.Height / 30F); i++)
                {
                    _random.Next(rect.Width);
                    _random.Next(rect.Height);
                    _random.Next(m / 50);
                    _random.Next(m / 50);
                }
            }

            // Clean up.
            path.Dispose();
            format.Dispose();
            font.Dispose();
            g.Dispose();

            return bitmap;
        }
    }
}
