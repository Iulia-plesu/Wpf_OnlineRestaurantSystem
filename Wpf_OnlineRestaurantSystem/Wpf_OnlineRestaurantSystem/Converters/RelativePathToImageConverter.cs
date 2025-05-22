using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Wpf_OnlineRestaurantSystem.ViewModels;
using System.Diagnostics;
using System.Windows;
namespace Wpf_OnlineRestaurantSystem.Converters
{
    public class RelativePathToImageConverter : IValueConverter
    {
        private static readonly string BaseImageDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debug.WriteLine($"Converter called with value: {value}");

            if (value is string imagePath)
            {
                try
                {
                    // Debug output
                    Debug.WriteLine($"Base directory: {BaseImageDirectory}");
                    Debug.WriteLine($"Directory exists: {Directory.Exists(BaseImageDirectory)}");

                    // Try absolute path first
                    if (Path.IsPathRooted(imagePath))
                    {
                        Debug.WriteLine($"Trying absolute path: {imagePath}");
                        if (File.Exists(imagePath))
                        {
                            return LoadImage(imagePath);
                        }
                    }

                    // Try with base directory
                    string fullPath = Path.Combine(BaseImageDirectory, imagePath);
                    Debug.WriteLine($"Trying path: {fullPath}");

                    if (File.Exists(fullPath))
                    {
                        return LoadImage(fullPath);
                    }

                    // Try with just filename
                    string fileName = Path.GetFileName(imagePath);
                    fullPath = Path.Combine(BaseImageDirectory, fileName);
                    Debug.WriteLine($"Trying filename only: {fullPath}");

                    if (File.Exists(fullPath))
                    {
                        return LoadImage(fullPath);
                    }

                    Debug.WriteLine("No valid image path found");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error loading image: {ex.Message}");
                }
            }
            return null;
        }

        private BitmapImage LoadImage(string path)
        {
            Debug.WriteLine($"Loading image from: {path}");
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(path);
            image.EndInit();
            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
