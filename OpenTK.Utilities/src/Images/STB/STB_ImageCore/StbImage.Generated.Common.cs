// Generated by Sichem at 12/24/2021 8:28:15 PM

using System.Runtime.InteropServices;
using Hebron.Runtime;

namespace OpenTK.Utilities.Images
{
	unsafe partial class ImageCore
	{
		public const int STBI__SCAN_header = 2;
		public const int STBI__SCAN_load = 0;
		public const int STBI__SCAN_type = 1;
		public const int STBI_default = 0;
		public const int STBI_grey = 1;
		public const int STBI_grey_alpha = 2;
		public const int STBI_ORDER_BGR = 1;
		public const int STBI_ORDER_RGB = 0;
		public const int STBI_rgb = 3;
		public const int STBI_rgb_alpha = 4;

		public static byte[] ComputeHuffmanCodesLengthDezigzag =
			{16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15};

		public static int DeIphoneFlagGlobal;
		public static int DeIphoneFlagLocal;
		public static int DeIphoneFlagSet;
		public static float H2lGammaI = 1.0f / 2.2f;
		public static float H2lScaleI = 1.0f;
		public static float L2hGamma = 2.2f;
		public static float L2hScale = 1.0f;
		public static byte[] ProcessFrameHeaderRgb = { 82, 71, 66 };
		public static byte[] ProcessMarkerTag = { 65, 100, 111, 98, 101, 0 };
		public static int[] ShiftsignedMulTable = { 0, 0xff, 0x55, 0x49, 0x11, 0x21, 0x41, 0x81, 0x01 };
		public static int[] ShiftsignedShiftTable = { 0, 0, 0, 1, 0, 2, 4, 6, 0 };
		public static int UnpremultiplyOnLoadGlobal;
		public static int UnpremultiplyOnLoadLocal;
		public static int UnpremultiplyOnLoadSet;
		public static int VerticallyFlipOnLoadGlobal;
		public static int VerticallyFlipOnLoadLocal;
		public static int VerticallyFlipOnLoadSet;

		public static void HdrToLdrGamma(float gamma)
		{
			H2lGammaI = 1 / gamma;
		}

		public static void HdrToLdrScale(float scale)
		{
			H2lScaleI = 1 / scale;
		}

		public static void LdrToHdrGamma(float gamma)
		{
			L2hGamma = gamma;
		}

		public static void LdrToHdrScale(float scale)
		{
			L2hScale = scale;
		}

		public static void SetUnpremultiplyOnLoad(int flag_true_if_should_unpremultiply)
		{
			UnpremultiplyOnLoadGlobal = flag_true_if_should_unpremultiply;
		}

		public static void ConvertIphonePngToRgb(int flag_true_if_should_convert)
		{
			DeIphoneFlagGlobal = flag_true_if_should_convert;
		}

		public static void SetFlipVerticallyOnLoad(int flag_true_if_should_flip)
		{
			VerticallyFlipOnLoadGlobal = flag_true_if_should_flip;
		}

		public static void ConvertIphonePngToRgbThread(int flag_true_if_should_convert)
		{
			DeIphoneFlagLocal = flag_true_if_should_convert;
			DeIphoneFlagSet = 1;
		}

		public static void SetFlipVerticallyOnLoadThread(int flag_true_if_should_flip)
		{
			VerticallyFlipOnLoadLocal = flag_true_if_should_flip;
			VerticallyFlipOnLoadSet = 1;
		}

		public static void* Malloc(ulong size)
		{
			return CRuntime.malloc(size);
		}

		public static int AddsizesValid(int a, int b)
		{
			if (b < 0)
				return 0;
			return a <= 2147483647 - b ? 1 : 0;
		}

		public static int Mul2sizesValid(int a, int b)
		{
			if (a < 0 || b < 0)
				return 0;
			if (b == 0)
				return 1;
			return a <= 2147483647 / b ? 1 : 0;
		}

		public static int Mad2sizesValid(int a, int b, int add)
		{
			return Mul2sizesValid(a, b) != 0 && AddsizesValid(a * b, add) != 0 ? 1 : 0;
		}

		public static int Mad3sizesValid(int a, int b, int c, int add)
		{
			return Mul2sizesValid(a, b) != 0 && Mul2sizesValid(a * b, c) != 0 &&
				   AddsizesValid(a * b * c, add) != 0
				? 1
				: 0;
		}

		public static int Mad4sizesValid(int a, int b, int c, int d, int add)
		{
			return Mul2sizesValid(a, b) != 0 && Mul2sizesValid(a * b, c) != 0 &&
				   Mul2sizesValid(a * b * c, d) != 0 && AddsizesValid(a * b * c * d, add) != 0
				? 1
				: 0;
		}

		public static void* MallocMad2(int a, int b, int add)
		{
			if (Mad2sizesValid(a, b, add) == 0)
				return null;
			return Malloc((ulong)(a * b + add));
		}

		public static void* MallocMad3(int a, int b, int c, int add)
		{
			if (Mad3sizesValid(a, b, c, add) == 0)
				return null;
			return Malloc((ulong)(a * b * c + add));
		}

		public static void* MallocMad4(int a, int b, int c, int d, int add)
		{
			if (Mad4sizesValid(a, b, c, d, add) == 0)
				return null;
			return Malloc((ulong)(a * b * c * d + add));
		}

		public static float* LdrToHdr(byte* data, int x, int y, int comp)
		{
			var i = 0;
			var k = 0;
			var n = 0;
			float* output;
			if (data == null)
				return null;
			output = (float*)MallocMad4(x, y, comp, sizeof(float), 0);
			if (output == null)
			{
				CRuntime.free(data);
				return (float*)(ulong)(Err("outofmem") != 0 ? 0 : 0);
			}

			if ((comp & 1) != 0)
				n = comp;
			else
				n = comp - 1;
			for (i = 0; i < x * y; ++i)
				for (k = 0; k < n; ++k)
					output[i * comp + k] =
						(float)(CRuntime.pow(data[i * comp + k] / 255.0f, L2hGamma) * L2hScale);
			if (n < comp)
				for (i = 0; i < x * y; ++i)
					output[i * comp + n] = data[i * comp + n] / 255.0f;
			CRuntime.free(data);
			return output;
		}

		public static void* LoadMain(Context s, int* x, int* y, int* comp, int req_comp,
			ResultInfo* ri, int bpc)
		{
			CRuntime.memset(ri, 0, (ulong)sizeof(ResultInfo));
			ri->bits_per_channel = 8;
			ri->channel_order = STBI_ORDER_RGB;
			ri->num_channels = 0;
			if (PngTest(s) != 0)
				return PngLoad(s, x, y, comp, req_comp, ri);
			if (BmpTest(s) != 0)
				return BmpLoad(s, x, y, comp, req_comp, ri);
			if (GifTest(s) != 0)
				return GifLoad(s, x, y, comp, req_comp, ri);
			if (PsdTest(s) != 0)
				return PsdLoad(s, x, y, comp, req_comp, ri, bpc);
			if (JpegTest(s) != 0)
				return JpegLoad(s, x, y, comp, req_comp, ri);
			if (HdrTest(s) != 0)
			{
				var hdr = HdrLoad(s, x, y, comp, req_comp, ri);
				return HdrToLdr(hdr, *x, *y, req_comp != 0 ? req_comp : *comp);
			}

			if (TgaTest(s) != 0)
				return TgaLoad(s, x, y, comp, req_comp, ri);
			return (byte*)(ulong)(Err("unknown image type") != 0 ? 0 : 0);
		}

		public static byte* Convert16To8(ushort* orig, int w, int h, int channels)
		{
			var i = 0;
			var img_len = w * h * channels;
			byte* reduced;
			reduced = (byte*)Malloc((ulong)img_len);
			if (reduced == null)
				return (byte*)(ulong)(Err("outofmem") != 0 ? 0 : 0);
			for (i = 0; i < img_len; ++i)
				reduced[i] = (byte)((orig[i] >> 8) & 0xFF);
			CRuntime.free(orig);
			return reduced;
		}

		public static ushort* Convert8To16(byte* orig, int w, int h, int channels)
		{
			var i = 0;
			var img_len = w * h * channels;
			ushort* enlarged;
			enlarged = (ushort*)Malloc((ulong)(img_len * 2));
			if (enlarged == null)
				return (ushort*)(byte*)(ulong)(Err("outofmem") != 0 ? 0 : 0);
			for (i = 0; i < img_len; ++i)
				enlarged[i] = (ushort)((orig[i] << 8) + orig[i]);
			CRuntime.free(orig);
			return enlarged;
		}

		public static void VerticalFlip(void* image, int w, int h, int bytes_per_pixel)
		{
			var row = 0;
			var bytes_per_row = w * bytes_per_pixel;
			var temp = stackalloc byte[2048];
			var bytes = (byte*)image;
			for (row = 0; row < h >> 1; row++)
			{
				var row0 = bytes + row * bytes_per_row;
				var row1 = bytes + (h - row - 1) * bytes_per_row;
				var bytes_left = (ulong)bytes_per_row;
				while (bytes_left != 0)
				{
					var bytes_copy = bytes_left < 2048 * sizeof(byte) ? bytes_left : 2048 * sizeof(byte);
					CRuntime.memcpy(temp, row0, bytes_copy);
					CRuntime.memcpy(row0, row1, bytes_copy);
					CRuntime.memcpy(row1, temp, bytes_copy);
					row0 += bytes_copy;
					row1 += bytes_copy;
					bytes_left -= bytes_copy;
				}
			}
		}

		public static void VerticalFlipSlices(void* image, int w, int h, int z, int bytes_per_pixel)
		{
			var slice = 0;
			var slice_size = w * h * bytes_per_pixel;
			var bytes = (byte*)image;
			for (slice = 0; slice < z; ++slice)
			{
				VerticalFlip(bytes, w, h, bytes_per_pixel);
				bytes += slice_size;
			}
		}

		public static byte* LoadAndPostprocess8bit(Context s, int* x, int* y, int* comp, int req_comp)
		{
			var ri = new ResultInfo();
			var result = LoadMain(s, x, y, comp, req_comp, &ri, 8);
			if (result == null)
				return null;
			if (ri.bits_per_channel != 8)
			{
				result = Convert16To8((ushort*)result, *x, *y, req_comp == 0 ? *comp : req_comp);
				ri.bits_per_channel = 8;
			}

			if ((VerticallyFlipOnLoadSet != 0
				? VerticallyFlipOnLoadLocal
				: VerticallyFlipOnLoadGlobal) != 0)
			{
				var channels = req_comp != 0 ? req_comp : *comp;
				VerticalFlip(result, *x, *y, channels * sizeof(byte));
			}

			return (byte*)result;
		}

		public static ushort* LoadAndPostprocess16bit(Context s, int* x, int* y, int* comp, int req_comp)
		{
			var ri = new ResultInfo();
			var result = LoadMain(s, x, y, comp, req_comp, &ri, 16);
			if (result == null)
				return null;
			if (ri.bits_per_channel != 16)
			{
				result = Convert8To16((byte*)result, *x, *y, req_comp == 0 ? *comp : req_comp);
				ri.bits_per_channel = 16;
			}

			if ((VerticallyFlipOnLoadSet != 0
				? VerticallyFlipOnLoadLocal
				: VerticallyFlipOnLoadGlobal) != 0)
			{
				var channels = req_comp != 0 ? req_comp : *comp;
				VerticalFlip(result, *x, *y, channels * sizeof(ushort));
			}

			return (ushort*)result;
		}

		public static void FloatPostprocess(float* result, int* x, int* y, int* comp, int req_comp)
		{
			if ((VerticallyFlipOnLoadSet != 0
				? VerticallyFlipOnLoadLocal
				: VerticallyFlipOnLoadGlobal) != 0 && result != null)
			{
				var channels = req_comp != 0 ? req_comp : *comp;
				VerticalFlip(result, *x, *y, channels * sizeof(float));
			}
		}

		public static float* LoadfMain(Context s, int* x, int* y, int* comp, int req_comp)
		{
			byte* data;
			if (HdrTest(s) != 0)
			{
				var ri = new ResultInfo();
				var hdr_data = HdrLoad(s, x, y, comp, req_comp, &ri);
				if (hdr_data != null)
					FloatPostprocess(hdr_data, x, y, comp, req_comp);
				return hdr_data;
			}

			data = LoadAndPostprocess8bit(s, x, y, comp, req_comp);
			if (data != null)
				return LdrToHdr(data, *x, *y, req_comp != 0 ? req_comp : *comp);
			return (float*)(ulong)(Err("unknown image type") != 0 ? 0 : 0);
		}

		public static int Get16be(Context s)
		{
			int z = Get8(s);
			return (z << 8) + Get8(s);
		}

		public static uint Get32be(Context s)
		{
			var z = (uint)Get16be(s);
			return (uint)((z << 16) + Get16be(s));
		}

		public static int Get16le(Context s)
		{
			int z = Get8(s);
			return z + (Get8(s) << 8);
		}

		public static uint Get32le(Context s)
		{
			var z = (uint)Get16le(s);
			z += (uint)Get16le(s) << 16;
			return z;
		}

		public static byte ComputeY(int r, int g, int b)
		{
			return (byte)((r * 77 + g * 150 + 29 * b) >> 8);
		}

		public static byte* ConvertFormat(byte* data, int img_n, int req_comp, uint x, uint y)
		{
			var i = 0;
			var j = 0;
			byte* good;
			if (req_comp == img_n)
				return data;
			good = (byte*)MallocMad3(req_comp, (int)x, (int)y, 0);
			if (good == null)
			{
				CRuntime.free(data);
				return (byte*)(ulong)(Err("outofmem") != 0 ? 0 : 0);
			}

			for (j = 0; j < (int)y; ++j)
			{
				var src = data + j * x * img_n;
				var dest = good + j * x * req_comp;
				switch (img_n * 8 + req_comp)
				{
					case 1 * 8 + 2:
						for (i = (int)(x - 1); i >= 0; --i, src += 1, dest += 2)
						{
							dest[0] = src[0];
							dest[1] = 255;
						}

						break;
					case 1 * 8 + 3:
						for (i = (int)(x - 1); i >= 0; --i, src += 1, dest += 3)
							dest[0] = dest[1] = dest[2] = src[0];
						break;
					case 1 * 8 + 4:
						for (i = (int)(x - 1); i >= 0; --i, src += 1, dest += 4)
						{
							dest[0] = dest[1] = dest[2] = src[0];
							dest[3] = 255;
						}

						break;
					case 2 * 8 + 1:
						for (i = (int)(x - 1); i >= 0; --i, src += 2, dest += 1)
							dest[0] = src[0];
						break;
					case 2 * 8 + 3:
						for (i = (int)(x - 1); i >= 0; --i, src += 2, dest += 3)
							dest[0] = dest[1] = dest[2] = src[0];
						break;
					case 2 * 8 + 4:
						for (i = (int)(x - 1); i >= 0; --i, src += 2, dest += 4)
						{
							dest[0] = dest[1] = dest[2] = src[0];
							dest[3] = src[1];
						}

						break;
					case 3 * 8 + 4:
						for (i = (int)(x - 1); i >= 0; --i, src += 3, dest += 4)
						{
							dest[0] = src[0];
							dest[1] = src[1];
							dest[2] = src[2];
							dest[3] = 255;
						}

						break;
					case 3 * 8 + 1:
						for (i = (int)(x - 1); i >= 0; --i, src += 3, dest += 1)
							dest[0] = ComputeY(src[0], src[1], src[2]);
						break;
					case 3 * 8 + 2:
						for (i = (int)(x - 1); i >= 0; --i, src += 3, dest += 2)
						{
							dest[0] = ComputeY(src[0], src[1], src[2]);
							dest[1] = 255;
						}

						break;
					case 4 * 8 + 1:
						for (i = (int)(x - 1); i >= 0; --i, src += 4, dest += 1)
							dest[0] = ComputeY(src[0], src[1], src[2]);
						break;
					case 4 * 8 + 2:
						for (i = (int)(x - 1); i >= 0; --i, src += 4, dest += 2)
						{
							dest[0] = ComputeY(src[0], src[1], src[2]);
							dest[1] = src[3];
						}

						break;
					case 4 * 8 + 3:
						for (i = (int)(x - 1); i >= 0; --i, src += 4, dest += 3)
						{
							dest[0] = src[0];
							dest[1] = src[1];
							dest[2] = src[2];
						}

						break;
					default:
						;
						CRuntime.free(data);
						CRuntime.free(good);
						return (byte*)(ulong)(Err("unsupported") != 0 ? 0 : 0);
				}
			}

			CRuntime.free(data);
			return good;
		}

		public static ushort ComputeY16(int r, int g, int b)
		{
			return (ushort)((r * 77 + g * 150 + 29 * b) >> 8);
		}

		public static ushort* ConvertFormat16(ushort* data, int img_n, int req_comp, uint x, uint y)
		{
			var i = 0;
			var j = 0;
			ushort* good;
			if (req_comp == img_n)
				return data;
			good = (ushort*)Malloc((ulong)(req_comp * x * y * 2));
			if (good == null)
			{
				CRuntime.free(data);
				return (ushort*)(byte*)(ulong)(Err("outofmem") != 0 ? 0 : 0);
			}

			for (j = 0; j < (int)y; ++j)
			{
				var src = data + j * x * img_n;
				var dest = good + j * x * req_comp;
				switch (img_n * 8 + req_comp)
				{
					case 1 * 8 + 2:
						for (i = (int)(x - 1); i >= 0; --i, src += 1, dest += 2)
						{
							dest[0] = src[0];
							dest[1] = 0xffff;
						}

						break;
					case 1 * 8 + 3:
						for (i = (int)(x - 1); i >= 0; --i, src += 1, dest += 3)
							dest[0] = dest[1] = dest[2] = src[0];
						break;
					case 1 * 8 + 4:
						for (i = (int)(x - 1); i >= 0; --i, src += 1, dest += 4)
						{
							dest[0] = dest[1] = dest[2] = src[0];
							dest[3] = 0xffff;
						}

						break;
					case 2 * 8 + 1:
						for (i = (int)(x - 1); i >= 0; --i, src += 2, dest += 1)
							dest[0] = src[0];
						break;
					case 2 * 8 + 3:
						for (i = (int)(x - 1); i >= 0; --i, src += 2, dest += 3)
							dest[0] = dest[1] = dest[2] = src[0];
						break;
					case 2 * 8 + 4:
						for (i = (int)(x - 1); i >= 0; --i, src += 2, dest += 4)
						{
							dest[0] = dest[1] = dest[2] = src[0];
							dest[3] = src[1];
						}

						break;
					case 3 * 8 + 4:
						for (i = (int)(x - 1); i >= 0; --i, src += 3, dest += 4)
						{
							dest[0] = src[0];
							dest[1] = src[1];
							dest[2] = src[2];
							dest[3] = 0xffff;
						}

						break;
					case 3 * 8 + 1:
						for (i = (int)(x - 1); i >= 0; --i, src += 3, dest += 1)
							dest[0] = ComputeY16(src[0], src[1], src[2]);
						break;
					case 3 * 8 + 2:
						for (i = (int)(x - 1); i >= 0; --i, src += 3, dest += 2)
						{
							dest[0] = ComputeY16(src[0], src[1], src[2]);
							dest[1] = 0xffff;
						}

						break;
					case 4 * 8 + 1:
						for (i = (int)(x - 1); i >= 0; --i, src += 4, dest += 1)
							dest[0] = ComputeY16(src[0], src[1], src[2]);
						break;
					case 4 * 8 + 2:
						for (i = (int)(x - 1); i >= 0; --i, src += 4, dest += 2)
						{
							dest[0] = ComputeY16(src[0], src[1], src[2]);
							dest[1] = src[3];
						}

						break;
					case 4 * 8 + 3:
						for (i = (int)(x - 1); i >= 0; --i, src += 4, dest += 3)
						{
							dest[0] = src[0];
							dest[1] = src[1];
							dest[2] = src[2];
						}

						break;
					default:
						;
						CRuntime.free(data);
						CRuntime.free(good);
						return (ushort*)(byte*)(ulong)(Err("unsupported") != 0 ? 0 : 0);
				}
			}

			CRuntime.free(data);
			return good;
		}

		public static byte Clamp(int x)
		{
			if ((uint)x > 255)
			{
				if (x < 0)
					return 0;
				if (x > 255)
					return 255;
			}

			return (byte)x;
		}

		public static byte Blinn8x8(byte x, byte y)
		{
			var t = (uint)(x * y + 128);
			return (byte)((t + (t >> 8)) >> 8);
		}

		public static int Bitreverse16(int n)
		{
			n = ((n & 0xAAAA) >> 1) | ((n & 0x5555) << 1);
			n = ((n & 0xCCCC) >> 2) | ((n & 0x3333) << 2);
			n = ((n & 0xF0F0) >> 4) | ((n & 0x0F0F) << 4);
			n = ((n & 0xFF00) >> 8) | ((n & 0x00FF) << 8);
			return n;
		}

		public static int BitReverse(int v, int bits)
		{
			return Bitreverse16(v) >> (16 - bits);
		}

		public static int Paeth(int a, int b, int c)
		{
			var p = a + b - c;
			var pa = CRuntime.abs(p - a);
			var pb = CRuntime.abs(p - b);
			var pc = CRuntime.abs(p - c);
			if (pa <= pb && pa <= pc)
				return a;
			if (pb <= pc)
				return b;
			return c;
		}

		public static void UnpremultiplyOnLoadThread(int flag_true_if_should_unpremultiply)
		{
			UnpremultiplyOnLoadLocal = flag_true_if_should_unpremultiply;
			UnpremultiplyOnLoadSet = 1;
		}

		public static int HighBit(uint z)
		{
			var n = 0;
			if (z == 0)
				return -1;
			if (z >= 0x10000)
			{
				n += 16;
				z >>= 16;
			}

			if (z >= 0x00100)
			{
				n += 8;
				z >>= 8;
			}

			if (z >= 0x00010)
			{
				n += 4;
				z >>= 4;
			}

			if (z >= 0x00004)
			{
				n += 2;
				z >>= 2;
			}

			if (z >= 0x00002)
				n += 1;
			return n;
		}

		public static int Bitcount(uint a)
		{
			a = (a & 0x55555555) + ((a >> 1) & 0x55555555);
			a = (a & 0x33333333) + ((a >> 2) & 0x33333333);
			a = (a + (a >> 4)) & 0x0f0f0f0f;
			a = a + (a >> 8);
			a = a + (a >> 16);
			return (int)(a & 0xff);
		}

		public static int Shiftsigned(uint v, int shift, int bits)
		{
			if (shift < 0)
				v <<= -shift;
			else
				v >>= shift;
			v >>= 8 - bits;
			return (int)(v * ShiftsignedMulTable[bits]) >> ShiftsignedShiftTable[bits];
		}

		public static int InfoMain(Context s, int* x, int* y, int* comp)
		{
			if (JpegInfo(s, x, y, comp) != 0)
				return 1;
			if (PngInfo(s, x, y, comp) != 0)
				return 1;
			if (GifInfo(s, x, y, comp) != 0)
				return 1;
			if (BmpInfo(s, x, y, comp) != 0)
				return 1;
			if (PsdInfo(s, x, y, comp) != 0)
				return 1;
			if (HdrInfo(s, x, y, comp) != 0)
				return 1;
			if (TgaInfo(s, x, y, comp) != 0)
				return 1;
			return Err("unknown image type");
		}

		public static int Is16Main(Context s)
		{
			if (PngIs16(s) != 0)
				return 1;
			if (PsdIs16(s) != 0)
				return 1;
			return 0;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ResultInfo
		{
			public int bits_per_channel;
			public int num_channels;
			public int channel_order;
		}
	}
}