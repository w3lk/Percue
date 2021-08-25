using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Percue.Extensions
{
	[Serializable]
	public class SerializedBitmapImage
	{
		public byte[] _imagePixelData;
		public int _imagePixelWidth;
		public int _imagePixelHeight;
		public double _imageDpiX;
		public double _imageDpiY;
		public string _imagePixelFormat;

		public SerializedBitmapImage() { }

		public void Serialize(BitmapImage bitmapImage)
		{
			_imagePixelFormat = bitmapImage.Format.ToString();
			_imagePixelWidth = bitmapImage.PixelWidth;
			_imagePixelHeight = bitmapImage.PixelHeight;
			_imageDpiX = bitmapImage.DpiX;
			_imageDpiY = bitmapImage.DpiY;

			int stride = (bitmapImage.Format.BitsPerPixel / 8) * bitmapImage.PixelWidth;
			int bytes = stride * bitmapImage.PixelHeight;

			_imagePixelData = new byte[bytes];
			bitmapImage.CopyPixels(_imagePixelData, stride, 0);
		}

		public BitmapSource Deserialize()
		{
			PixelFormat pixelFormat = (PixelFormat)new PixelFormatConverter().ConvertFromString(_imagePixelFormat);
			int stride = (pixelFormat.BitsPerPixel / 8) * _imagePixelWidth;

			return BitmapSource.Create(_imagePixelWidth, _imagePixelHeight, _imageDpiX,
				_imageDpiY, pixelFormat, null, _imagePixelData, stride);
		}
	}
}
