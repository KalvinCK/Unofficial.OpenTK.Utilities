// Generated by Sichem at 12/24/2021 8:28:15 PM

using System.Runtime.InteropServices;
using Hebron.Runtime;

namespace OpenTK.Utilities.Images
{
	unsafe partial class ImageCore
	{
		public const int STBI__F_avg = 3;
		public const int STBI__F_avg_first = 5;
		public const int STBI__F_none = 0;
		public const int STBI__F_paeth = 4;
		public const int STBI__F_paeth_first = 6;
		public const int STBI__F_sub = 1;
		public const int STBI__F_up = 2;

		public static byte[] first_row_filter =
			{STBI__F_none, STBI__F_sub, STBI__F_none, STBI__F_avg_first, STBI__F_paeth_first};

		public static byte[] CheckPngHeaderPngSig = { 137, 80, 78, 71, 13, 10, 26, 10 };
		public static byte[] DepthScaleTable = { 0, 0xff, 0x55, 0, 0x11, 0, 0, 0, 0x01 };

		public static int PngTest(Context s)
		{
			var r = 0;
			r = CheckPngHeader(s);
			Rewind(s);
			return r;
		}

		public static void* PngLoad(Context s, int* x, int* y, int* comp, int req_comp,
			ResultInfo* ri)
		{
			var p = new Png();
			p.s = s;
			return DoPng(p, x, y, comp, req_comp, ri);
		}

		public static int PngInfo(Context s, int* x, int* y, int* comp)
		{
			var p = new Png();
			p.s = s;
			return PngInfoRaw(p, x, y, comp);
		}

		public static int PngIs16(Context s)
		{
			var p = new Png();
			p.s = s;
			if (PngInfoRaw(p, null, null, null) == 0)
				return 0;
			if (p.depth != 16)
			{
				Rewind(p.s);
				return 0;
			}

			return 1;
		}

		public static Pngchunk GetChunkHeader(Context s)
		{
			var c = new Pngchunk();
			c.length = Get32be(s);
			c.type = Get32be(s);
			return c;
		}

		public static int CheckPngHeader(Context s)
		{
			var i = 0;
			for (i = 0; i < 8; ++i)
				if (Get8(s) != CheckPngHeaderPngSig[i])
					return Err("bad png sig");
			return 1;
		}

		public static int CreatePngImageRaw(Png a, byte* raw, uint raw_len, int out_n, uint x, uint y,
			int depth, int color)
		{
			var bytes = depth == 16 ? 2 : 1;
			var s = a.s;
			uint i = 0;
			uint j = 0;
			var stride = (uint)(x * out_n * bytes);
			uint img_len = 0;
			uint img_width_bytes = 0;
			var k = 0;
			var img_n = s.img_n;
			var output_bytes = out_n * bytes;
			var filter_bytes = img_n * bytes;
			var width = (int)x;
			a._out_ = (byte*)MallocMad3((int)x, (int)y, output_bytes, 0);
			if (a._out_ == null)
				return Err("outofmem");
			if (Mad3sizesValid(img_n, (int)x, depth, 7) == 0)
				return Err("too large");
			img_width_bytes = (uint)((img_n * x * depth + 7) >> 3);
			img_len = (img_width_bytes + 1) * y;
			if (raw_len < img_len)
				return Err("not enough pixels");
			for (j = 0; j < y; ++j)
			{
				var cur = a._out_ + stride * j;
				byte* prior;
				int filter = *raw++;
				if (filter > 4)
					return Err("invalid filter");
				if (depth < 8)
				{
					if (img_width_bytes > x)
						return Err("invalid width");
					cur += x * out_n - img_width_bytes;
					filter_bytes = 1;
					width = (int)img_width_bytes;
				}

				prior = cur - stride;
				if (j == 0)
					filter = first_row_filter[filter];
				for (k = 0; k < filter_bytes; ++k)
					switch (filter)
					{
						case STBI__F_none:
							cur[k] = raw[k];
							break;
						case STBI__F_sub:
							cur[k] = raw[k];
							break;
						case STBI__F_up:
							cur[k] = (byte)((raw[k] + prior[k]) & 255);
							break;
						case STBI__F_avg:
							cur[k] = (byte)((raw[k] + (prior[k] >> 1)) & 255);
							break;
						case STBI__F_paeth:
							cur[k] = (byte)((raw[k] + Paeth(0, prior[k], 0)) & 255);
							break;
						case STBI__F_avg_first:
							cur[k] = raw[k];
							break;
						case STBI__F_paeth_first:
							cur[k] = raw[k];
							break;
					}

				if (depth == 8)
				{
					if (img_n != out_n)
						cur[img_n] = 255;
					raw += img_n;
					cur += out_n;
					prior += out_n;
				}
				else if (depth == 16)
				{
					if (img_n != out_n)
					{
						cur[filter_bytes] = 255;
						cur[filter_bytes + 1] = 255;
					}

					raw += filter_bytes;
					cur += output_bytes;
					prior += output_bytes;
				}
				else
				{
					raw += 1;
					cur += 1;
					prior += 1;
				}

				if (depth < 8 || img_n == out_n)
				{
					var nk = (width - 1) * filter_bytes;
					switch (filter)
					{
						case STBI__F_none:
							CRuntime.memcpy(cur, raw, (ulong)nk);
							break;
						case STBI__F_sub:
							for (k = 0; k < nk; ++k)
								cur[k] = (byte)((raw[k] + cur[k - filter_bytes]) & 255);
							break;
						case STBI__F_up:
							for (k = 0; k < nk; ++k)
								cur[k] = (byte)((raw[k] + prior[k]) & 255);
							break;
						case STBI__F_avg:
							for (k = 0; k < nk; ++k)
								cur[k] = (byte)((raw[k] + ((prior[k] + cur[k - filter_bytes]) >> 1)) & 255);
							break;
						case STBI__F_paeth:
							for (k = 0; k < nk; ++k)
								cur[k] = (byte)((raw[k] + Paeth(cur[k - filter_bytes], prior[k],
									prior[k - filter_bytes])) & 255);
							break;
						case STBI__F_avg_first:
							for (k = 0; k < nk; ++k)
								cur[k] = (byte)((raw[k] + (cur[k - filter_bytes] >> 1)) & 255);
							break;
						case STBI__F_paeth_first:
							for (k = 0; k < nk; ++k)
								cur[k] = (byte)((raw[k] + Paeth(cur[k - filter_bytes], 0, 0)) & 255);
							break;
					}

					raw += nk;
				}
				else
				{
					switch (filter)
					{
						case STBI__F_none:
							for (i = x - 1;
								i >= 1;
								--i, cur[filter_bytes] = 255, raw += filter_bytes, cur += output_bytes, prior +=
									output_bytes)
								for (k = 0; k < filter_bytes; ++k)
									cur[k] = raw[k];
							break;
						case STBI__F_sub:
							for (i = x - 1;
								i >= 1;
								--i, cur[filter_bytes] = 255, raw += filter_bytes, cur += output_bytes, prior +=
									output_bytes)
								for (k = 0; k < filter_bytes; ++k)
									cur[k] = (byte)((raw[k] + cur[k - output_bytes]) & 255);
							break;
						case STBI__F_up:
							for (i = x - 1;
								i >= 1;
								--i, cur[filter_bytes] = 255, raw += filter_bytes, cur += output_bytes, prior +=
									output_bytes)
								for (k = 0; k < filter_bytes; ++k)
									cur[k] = (byte)((raw[k] + prior[k]) & 255);
							break;
						case STBI__F_avg:
							for (i = x - 1;
								i >= 1;
								--i, cur[filter_bytes] = 255, raw += filter_bytes, cur += output_bytes, prior +=
									output_bytes)
								for (k = 0; k < filter_bytes; ++k)
									cur[k] = (byte)((raw[k] + ((prior[k] + cur[k - output_bytes]) >> 1)) & 255);
							break;
						case STBI__F_paeth:
							for (i = x - 1;
								i >= 1;
								--i, cur[filter_bytes] = 255, raw += filter_bytes, cur += output_bytes, prior +=
									output_bytes)
								for (k = 0; k < filter_bytes; ++k)
									cur[k] = (byte)((raw[k] + Paeth(cur[k - output_bytes], prior[k],
										prior[k - output_bytes])) & 255);
							break;
						case STBI__F_avg_first:
							for (i = x - 1;
								i >= 1;
								--i, cur[filter_bytes] = 255, raw += filter_bytes, cur += output_bytes, prior +=
									output_bytes)
								for (k = 0; k < filter_bytes; ++k)
									cur[k] = (byte)((raw[k] + (cur[k - output_bytes] >> 1)) & 255);
							break;
						case STBI__F_paeth_first:
							for (i = x - 1;
								i >= 1;
								--i, cur[filter_bytes] = 255, raw += filter_bytes, cur += output_bytes, prior +=
									output_bytes)
								for (k = 0; k < filter_bytes; ++k)
									cur[k] = (byte)((raw[k] + Paeth(cur[k - output_bytes], 0, 0)) & 255);
							break;
					}

					if (depth == 16)
					{
						cur = a._out_ + stride * j;
						for (i = 0; i < x; ++i, cur += output_bytes)
							cur[filter_bytes + 1] = 255;
					}
				}
			}

			if (depth < 8)
			{
				for (j = 0; j < y; ++j)
				{
					var cur = a._out_ + stride * j;
					var _in_ = a._out_ + stride * j + x * out_n - img_width_bytes;
					var scale = (byte)(color == 0 ? DepthScaleTable[depth] : 1);
					if (depth == 4)
					{
						for (k = (int)(x * img_n); k >= 2; k -= 2, ++_in_)
						{
							*cur++ = (byte)(scale * (*_in_ >> 4));
							*cur++ = (byte)(scale * (*_in_ & 0x0f));
						}

						if (k > 0)
							*cur++ = (byte)(scale * (*_in_ >> 4));
					}
					else if (depth == 2)
					{
						for (k = (int)(x * img_n); k >= 4; k -= 4, ++_in_)
						{
							*cur++ = (byte)(scale * (*_in_ >> 6));
							*cur++ = (byte)(scale * ((*_in_ >> 4) & 0x03));
							*cur++ = (byte)(scale * ((*_in_ >> 2) & 0x03));
							*cur++ = (byte)(scale * (*_in_ & 0x03));
						}

						if (k > 0)
							*cur++ = (byte)(scale * (*_in_ >> 6));
						if (k > 1)
							*cur++ = (byte)(scale * ((*_in_ >> 4) & 0x03));
						if (k > 2)
							*cur++ = (byte)(scale * ((*_in_ >> 2) & 0x03));
					}
					else if (depth == 1)
					{
						for (k = (int)(x * img_n); k >= 8; k -= 8, ++_in_)
						{
							*cur++ = (byte)(scale * (*_in_ >> 7));
							*cur++ = (byte)(scale * ((*_in_ >> 6) & 0x01));
							*cur++ = (byte)(scale * ((*_in_ >> 5) & 0x01));
							*cur++ = (byte)(scale * ((*_in_ >> 4) & 0x01));
							*cur++ = (byte)(scale * ((*_in_ >> 3) & 0x01));
							*cur++ = (byte)(scale * ((*_in_ >> 2) & 0x01));
							*cur++ = (byte)(scale * ((*_in_ >> 1) & 0x01));
							*cur++ = (byte)(scale * (*_in_ & 0x01));
						}

						if (k > 0)
							*cur++ = (byte)(scale * (*_in_ >> 7));
						if (k > 1)
							*cur++ = (byte)(scale * ((*_in_ >> 6) & 0x01));
						if (k > 2)
							*cur++ = (byte)(scale * ((*_in_ >> 5) & 0x01));
						if (k > 3)
							*cur++ = (byte)(scale * ((*_in_ >> 4) & 0x01));
						if (k > 4)
							*cur++ = (byte)(scale * ((*_in_ >> 3) & 0x01));
						if (k > 5)
							*cur++ = (byte)(scale * ((*_in_ >> 2) & 0x01));
						if (k > 6)
							*cur++ = (byte)(scale * ((*_in_ >> 1) & 0x01));
					}

					if (img_n != out_n)
					{
						var q = 0;
						cur = a._out_ + stride * j;
						if (img_n == 1)
							for (q = (int)(x - 1); q >= 0; --q)
							{
								cur[q * 2 + 1] = 255;
								cur[q * 2 + 0] = cur[q];
							}
						else
							for (q = (int)(x - 1); q >= 0; --q)
							{
								cur[q * 4 + 3] = 255;
								cur[q * 4 + 2] = cur[q * 3 + 2];
								cur[q * 4 + 1] = cur[q * 3 + 1];
								cur[q * 4 + 0] = cur[q * 3 + 0];
							}
					}
				}
			}
			else if (depth == 16)
			{
				var cur = a._out_;
				var cur16 = (ushort*)cur;
				for (i = 0; i < x * y * out_n; ++i, cur16++, cur += 2)
					*cur16 = (ushort)((cur[0] << 8) | cur[1]);
			}

			return 1;
		}

		public static int CreatePngImage(Png a, byte* image_data, uint image_data_len, int out_n,
			int depth, int color, int interlaced)
		{
			var bytes = depth == 16 ? 2 : 1;
			var out_bytes = out_n * bytes;
			byte* final;
			var p = 0;
			if (interlaced == 0)
				return CreatePngImageRaw(a, image_data, image_data_len, out_n, a.s.img_x, a.s.img_y, depth,
					color);
			final = (byte*)MallocMad3((int)a.s.img_x, (int)a.s.img_y, out_bytes, 0);
			if (final == null)
				return Err("outofmem");
			for (p = 0; p < 7; ++p)
			{
				var xorig = stackalloc int[] { 0, 4, 0, 2, 0, 1, 0 };
				var yorig = stackalloc int[] { 0, 0, 4, 0, 2, 0, 1 };
				var xspc = stackalloc int[] { 8, 8, 4, 4, 2, 2, 1 };
				var yspc = stackalloc int[] { 8, 8, 8, 4, 4, 2, 2 };
				var i = 0;
				var j = 0;
				var x = 0;
				var y = 0;
				x = (int)((a.s.img_x - xorig[p] + xspc[p] - 1) / xspc[p]);
				y = (int)((a.s.img_y - yorig[p] + yspc[p] - 1) / yspc[p]);
				if (x != 0 && y != 0)
				{
					var img_len = (uint)((((a.s.img_n * x * depth + 7) >> 3) + 1) * y);
					if (CreatePngImageRaw(a, image_data, image_data_len, out_n, (uint)x, (uint)y, depth,
						color) == 0)
					{
						CRuntime.free(final);
						return 0;
					}

					for (j = 0; j < y; ++j)
						for (i = 0; i < x; ++i)
						{
							var out_y = j * yspc[p] + yorig[p];
							var out_x = i * xspc[p] + xorig[p];
							CRuntime.memcpy(final + out_y * a.s.img_x * out_bytes + out_x * out_bytes,
								a._out_ + (j * x + i) * out_bytes, (ulong)out_bytes);
						}

					CRuntime.free(a._out_);
					image_data += img_len;
					image_data_len -= img_len;
				}
			}

			a._out_ = final;
			return 1;
		}

		public static int ComputeTransparency(Png z, byte* tc, int out_n)
		{
			var s = z.s;
			uint i = 0;
			var pixel_count = s.img_x * s.img_y;
			var p = z._out_;
			if (out_n == 2)
				for (i = 0; i < pixel_count; ++i)
				{
					p[1] = (byte)(p[0] == tc[0] ? 0 : 255);
					p += 2;
				}
			else
				for (i = 0; i < pixel_count; ++i)
				{
					if (p[0] == tc[0] && p[1] == tc[1] && p[2] == tc[2])
						p[3] = 0;
					p += 4;
				}

			return 1;
		}

		public static int ComputeTransparency16(Png z, ushort* tc, int out_n)
		{
			var s = z.s;
			uint i = 0;
			var pixel_count = s.img_x * s.img_y;
			var p = (ushort*)z._out_;
			if (out_n == 2)
				for (i = 0; i < pixel_count; ++i)
				{
					p[1] = (ushort)(p[0] == tc[0] ? 0 : 65535);
					p += 2;
				}
			else
				for (i = 0; i < pixel_count; ++i)
				{
					if (p[0] == tc[0] && p[1] == tc[1] && p[2] == tc[2])
						p[3] = 0;
					p += 4;
				}

			return 1;
		}

		public static int ExpandPngPalette(Png a, byte* palette, int len, int pal_img_n)
		{
			uint i = 0;
			var pixel_count = a.s.img_x * a.s.img_y;
			byte* p;
			byte* temp_out;
			var orig = a._out_;
			p = (byte*)MallocMad2((int)pixel_count, pal_img_n, 0);
			if (p == null)
				return Err("outofmem");
			temp_out = p;
			if (pal_img_n == 3)
				for (i = 0; i < pixel_count; ++i)
				{
					var n = orig[i] * 4;
					p[0] = palette[n];
					p[1] = palette[n + 1];
					p[2] = palette[n + 2];
					p += 3;
				}
			else
				for (i = 0; i < pixel_count; ++i)
				{
					var n = orig[i] * 4;
					p[0] = palette[n];
					p[1] = palette[n + 1];
					p[2] = palette[n + 2];
					p[3] = palette[n + 3];
					p += 4;
				}

			CRuntime.free(a._out_);
			a._out_ = temp_out;
			return 1;
		}

		public static void DeIphone(Png z)
		{
			var s = z.s;
			uint i = 0;
			var pixel_count = s.img_x * s.img_y;
			var p = z._out_;
			if (s.img_out_n == 3)
			{
				for (i = 0; i < pixel_count; ++i)
				{
					var t = p[0];
					p[0] = p[2];
					p[2] = t;
					p += 3;
				}
			}
			else
			{
				if ((UnpremultiplyOnLoadSet != 0
					? UnpremultiplyOnLoadLocal
					: UnpremultiplyOnLoadGlobal) != 0)
					for (i = 0; i < pixel_count; ++i)
					{
						var a = p[3];
						var t = p[0];
						if (a != 0)
						{
							var half = (byte)(a / 2);
							p[0] = (byte)((p[2] * 255 + half) / a);
							p[1] = (byte)((p[1] * 255 + half) / a);
							p[2] = (byte)((t * 255 + half) / a);
						}
						else
						{
							p[0] = p[2];
							p[2] = t;
						}

						p += 4;
					}
				else
					for (i = 0; i < pixel_count; ++i)
					{
						var t = p[0];
						p[0] = p[2];
						p[2] = t;
						p += 4;
					}
			}
		}

		public static int ParsePngFile(Png z, int scan, int req_comp)
		{
			var palette = stackalloc byte[1024];
			byte pal_img_n = 0;
			byte has_trans = 0;
			var tc = stackalloc byte[] { 0, 0, 0 };
			var tc16 = stackalloc ushort[3];
			uint ioff = 0;
			uint idata_limit = 0;
			uint i = 0;
			uint pal_len = 0;
			var first = 1;
			var k = 0;
			var interlace = 0;
			var color = 0;
			var is_iphone = 0;
			var s = z.s;
			z.expanded = null;
			z.idata = null;
			z._out_ = null;
			if (CheckPngHeader(s) == 0)
				return 0;
			if (scan == STBI__SCAN_type)
				return 1;
			for (; ; )
			{
				var c = GetChunkHeader(s);
				switch (c.type)
				{
					case ((uint)67 << 24) + ((uint)103 << 16) + ((uint)66 << 8) + 73:
						is_iphone = 1;
						Skip(s, (int)c.length);
						break;
					case ((uint)73 << 24) + ((uint)72 << 16) + ((uint)68 << 8) + 82:
						{
							var comp = 0;
							var filter = 0;
							if (first == 0)
								return Err("multiple IHDR");
							first = 0;
							if (c.length != 13)
								return Err("bad IHDR len");
							s.img_x = Get32be(s);
							s.img_y = Get32be(s);
							if (s.img_y > 1 << 24)
								return Err("too large");
							if (s.img_x > 1 << 24)
								return Err("too large");
							z.depth = Get8(s);
							if (z.depth != 1 && z.depth != 2 && z.depth != 4 && z.depth != 8 && z.depth != 16)
								return Err("1/2/4/8/16-bit only");
							color = Get8(s);
							if (color > 6)
								return Err("bad ctype");
							if (color == 3 && z.depth == 16)
								return Err("bad ctype");
							if (color == 3)
								pal_img_n = 3;
							else if ((color & 1) != 0)
								return Err("bad ctype");
							comp = Get8(s);
							if (comp != 0)
								return Err("bad comp method");
							filter = Get8(s);
							if (filter != 0)
								return Err("bad filter method");
							interlace = Get8(s);
							if (interlace > 1)
								return Err("bad interlace method");
							if (s.img_x == 0 || s.img_y == 0)
								return Err("0-pixel image");
							if (pal_img_n == 0)
							{
								s.img_n = ((color & 2) != 0 ? 3 : 1) + ((color & 4) != 0 ? 1 : 0);
								if ((1 << 30) / s.img_x / s.img_n < s.img_y)
									return Err("too large");
								if (scan == STBI__SCAN_header)
									return 1;
							}
							else
							{
								s.img_n = 1;
								if ((1 << 30) / s.img_x / 4 < s.img_y)
									return Err("too large");
							}

							break;
						}

					case ((uint)80 << 24) + ((uint)76 << 16) + ((uint)84 << 8) + 69:
						{
							if (first != 0)
								return Err("first not IHDR");
							if (c.length > 256 * 3)
								return Err("invalid PLTE");
							pal_len = c.length / 3;
							if (pal_len * 3 != c.length)
								return Err("invalid PLTE");
							for (i = 0; i < pal_len; ++i)
							{
								palette[i * 4 + 0] = Get8(s);
								palette[i * 4 + 1] = Get8(s);
								palette[i * 4 + 2] = Get8(s);
								palette[i * 4 + 3] = 255;
							}

							break;
						}

					case ((uint)116 << 24) + ((uint)82 << 16) + ((uint)78 << 8) + 83:
						{
							if (first != 0)
								return Err("first not IHDR");
							if (z.idata != null)
								return Err("tRNS after IDAT");
							if (pal_img_n != 0)
							{
								if (scan == STBI__SCAN_header)
								{
									s.img_n = 4;
									return 1;
								}

								if (pal_len == 0)
									return Err("tRNS before PLTE");
								if (c.length > pal_len)
									return Err("bad tRNS len");
								pal_img_n = 4;
								for (i = 0; i < c.length; ++i)
									palette[i * 4 + 3] = Get8(s);
							}
							else
							{
								if ((s.img_n & 1) == 0)
									return Err("tRNS with alpha");
								if (c.length != (uint)s.img_n * 2)
									return Err("bad tRNS len");
								has_trans = 1;
								if (z.depth == 16)
									for (k = 0; k < s.img_n; ++k)
										tc16[k] = (ushort)Get16be(s);
								else
									for (k = 0; k < s.img_n; ++k)
										tc[k] = (byte)((byte)(Get16be(s) & 255) * DepthScaleTable[z.depth]);
							}

							break;
						}

					case ((uint)73 << 24) + ((uint)68 << 16) + ((uint)65 << 8) + 84:
						{
							if (first != 0)
								return Err("first not IHDR");
							if (pal_img_n != 0 && pal_len == 0)
								return Err("no PLTE");
							if (scan == STBI__SCAN_header)
							{
								s.img_n = pal_img_n;
								return 1;
							}

							if ((int)(ioff + c.length) < (int)ioff)
								return 0;
							if (ioff + c.length > idata_limit)
							{
								var idata_limit_old = idata_limit;
								byte* p;
								if (idata_limit == 0)
									idata_limit = c.length > 4096 ? c.length : 4096;
								while (ioff + c.length > idata_limit)
									idata_limit *= 2;
								p = (byte*)CRuntime.realloc(z.idata, (ulong)idata_limit);
								if (p == null)
									return Err("outofmem");
								z.idata = p;
							}

							if (Getn(s, z.idata + ioff, (int)c.length) == 0)
								return Err("outofdata");
							ioff += c.length;
							break;
						}

					case ((uint)73 << 24) + ((uint)69 << 16) + ((uint)78 << 8) + 68:
						{
							uint raw_len = 0;
							uint bpl = 0;
							if (first != 0)
								return Err("first not IHDR");
							if (scan != STBI__SCAN_load)
								return 1;
							if (z.idata == null)
								return Err("no IDAT");
							bpl = (uint)((s.img_x * z.depth + 7) / 8);
							raw_len = (uint)(bpl * s.img_y * s.img_n + s.img_y);
							z.expanded = (byte*)ZlibDecodeMallocGuesssizeHeaderflag((sbyte*)z.idata, (int)ioff,
								(int)raw_len, (int*)&raw_len, is_iphone == 0 ? 1 : 0);
							if (z.expanded == null)
								return 0;
							CRuntime.free(z.idata);
							z.idata = null;
							if (req_comp == s.img_n + 1 && req_comp != 3 && pal_img_n == 0 || has_trans != 0)
								s.img_out_n = s.img_n + 1;
							else
								s.img_out_n = s.img_n;
							if (CreatePngImage(z, z.expanded, raw_len, s.img_out_n, z.depth, color, interlace) == 0)
								return 0;
							if (has_trans != 0)
							{
								if (z.depth == 16)
								{
									if (ComputeTransparency16(z, tc16, s.img_out_n) == 0)
										return 0;
								}
								else
								{
									if (ComputeTransparency(z, tc, s.img_out_n) == 0)
										return 0;
								}
							}

							if (is_iphone != 0 &&
								(DeIphoneFlagSet != 0
									? DeIphoneFlagLocal
									: DeIphoneFlagGlobal) != 0 && s.img_out_n > 2)
								DeIphone(z);
							if (pal_img_n != 0)
							{
								s.img_n = pal_img_n;
								s.img_out_n = pal_img_n;
								if (req_comp >= 3)
									s.img_out_n = req_comp;
								if (ExpandPngPalette(z, palette, (int)pal_len, s.img_out_n) == 0)
									return 0;
							}
							else if (has_trans != 0)
							{
								++s.img_n;
							}

							CRuntime.free(z.expanded);
							z.expanded = null;
							Get32be(s);
							return 1;
						}

					default:
						if (first != 0)
							return Err("first not IHDR");
						if ((c.type & (1 << 29)) == 0)
						{
							ParsePngFileInvalidChunk[0] = (char)((c.type >> 24) & 255);
							ParsePngFileInvalidChunk[1] = (char)((c.type >> 16) & 255);
							ParsePngFileInvalidChunk[2] = (char)((c.type >> 8) & 255);
							ParsePngFileInvalidChunk[3] = (char)((c.type >> 0) & 255);
							return Err(new string(ParsePngFileInvalidChunk));
						}

						Skip(s, (int)c.length);
						break;
				}

				Get32be(s);
			}
		}

		public static void* DoPng(Png p, int* x, int* y, int* n, int req_comp, ResultInfo* ri)
		{
			void* result = null;
			if (req_comp < 0 || req_comp > 4)
				return (byte*)(ulong)(Err("bad req_comp") != 0 ? 0 : 0);
			if (ParsePngFile(p, STBI__SCAN_load, req_comp) != 0)
			{
				if (p.depth <= 8)
					ri->bits_per_channel = 8;
				else if (p.depth == 16)
					ri->bits_per_channel = 16;
				else
					return (byte*)(ulong)(Err("bad bits_per_channel") != 0 ? 0 : 0);
				result = p._out_;
				p._out_ = null;
				if (req_comp != 0 && req_comp != p.s.img_out_n)
				{
					if (ri->bits_per_channel == 8)
						result = ConvertFormat((byte*)result, p.s.img_out_n, req_comp, p.s.img_x, p.s.img_y);
					else
						result = ConvertFormat16((ushort*)result, p.s.img_out_n, req_comp, p.s.img_x,
							p.s.img_y);
					p.s.img_out_n = req_comp;
					if (result == null)
						return result;
				}

				*x = (int)p.s.img_x;
				*y = (int)p.s.img_y;
				if (n != null)
					*n = p.s.img_n;
			}

			CRuntime.free(p._out_);
			p._out_ = null;
			CRuntime.free(p.expanded);
			p.expanded = null;
			CRuntime.free(p.idata);
			p.idata = null;
			return result;
		}

		public static int PngInfoRaw(Png p, int* x, int* y, int* comp)
		{
			if (ParsePngFile(p, STBI__SCAN_header, 0) == 0)
			{
				Rewind(p.s);
				return 0;
			}

			if (x != null)
				*x = (int)p.s.img_x;
			if (y != null)
				*y = (int)p.s.img_y;
			if (comp != null)
				*comp = p.s.img_n;
			return 1;
		}

		public class Png
		{
			public byte* _out_;
			public int depth;
			public byte* expanded;
			public byte* idata;
			public Context s;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct Pngchunk
		{
			public uint length;
			public uint type;
		}
	}
}
