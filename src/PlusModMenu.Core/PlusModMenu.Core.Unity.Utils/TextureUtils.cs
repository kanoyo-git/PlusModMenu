using UnityEngine;

namespace PlusModMenu.Core.Unity.Utils;

public class TextureUtils
{
	public enum Rotation
	{
		_90,
		_180,
		_270
	}

	public static Texture2D FlipX(Texture2D texture)
	{
		Texture2D texture2D = new Texture2D(texture.width, texture.height);
		int width = texture.width;
		int height = texture.height;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				texture2D.SetPixel(width - i - 1, j, texture.GetPixel(i, j));
			}
		}
		texture2D.Apply();
		return texture2D;
	}

	public static Texture2D FlipY(Texture2D texture)
	{
		Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, mipChain: false);
		int width = texture.width;
		int height = texture.height;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				texture2D.SetPixel(i, height - j - 1, texture.GetPixel(i, j));
			}
		}
		texture2D.Apply();
		return texture2D;
	}

	public static Texture2D Rotate(Texture2D texture, Rotation rotation)
	{
		Color32[] array = texture.GetPixels32();
		Color32[] array2 = new Color32[array.Length];
		int width = texture.width;
		int height = texture.height;
		for (int i = 0; i <= (int)rotation; i++)
		{
			array2 = new Color32[array.Length];
			for (int j = 0; j < height; j++)
			{
				for (int k = 0; k < width; k++)
				{
					int num = (k + 1) * height - j - 1;
					int num2 = array.Length - 1 - (j * width + k);
					array2[num] = array[num2];
				}
			}
			array = array2;
		}
		Texture2D texture2D = new Texture2D(height, width, TextureFormat.RGBA32, mipChain: false);
		texture2D.SetPixels32(array2);
		texture2D.Apply();
		return texture2D;
	}

	public static Texture2D InvertColors(Texture2D input)
	{
		Color32[] pixels = input.GetPixels32();
		for (int i = 0; i < pixels.Length; i++)
		{
			pixels[i].r = (byte)(~pixels[i].r);
			pixels[i].g = (byte)(~pixels[i].g);
			pixels[i].b = (byte)(~pixels[i].b);
		}
		Texture2D texture2D = new Texture2D(input.width, input.height, TextureFormat.RGBA32, mipChain: false);
		texture2D.SetPixels32(pixels);
		texture2D.Apply();
		return texture2D;
	}

	public static Texture2D CreateFromImageData(byte[] imageData, bool mipChain = false)
	{
		Texture2D texture2D = new Texture2D(1, 1, TextureFormat.RGBA32, mipChain);
		texture2D.LoadImage(imageData);
		return texture2D;
	}

	public static Texture2D CreateFromPixels(int width, int height, Color[] pixels)
	{
		Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGBA32, mipChain: false);
		texture2D.SetPixels(pixels);
		texture2D.Apply();
		return texture2D;
	}

	public static Texture2D CreateFromColor(Color color)
	{
		return CreateFromPixels(2, 2, new Color[4] { color, color, color, color });
	}

	public static Cubemap CubemapFromTexture2D(Texture2D texture)
	{
		texture = FlipX(texture);
		texture = FlipY(texture);
		int num = texture.width / 6;
		Cubemap cubemap = new Cubemap(num, TextureFormat.ARGB32, mipChain: false);
		cubemap.SetPixels(texture.GetPixels(0, 0, num, num), CubemapFace.NegativeZ);
		cubemap.SetPixels(texture.GetPixels(num, 0, num, num), CubemapFace.PositiveZ);
		cubemap.SetPixels(texture.GetPixels(2 * num, 0, num, num), CubemapFace.NegativeY);
		cubemap.SetPixels(texture.GetPixels(3 * num, 0, num, num), CubemapFace.PositiveY);
		cubemap.SetPixels(texture.GetPixels(4 * num, 0, num, num), CubemapFace.NegativeX);
		cubemap.SetPixels(texture.GetPixels(5 * num, 0, num, num), CubemapFace.PositiveX);
		cubemap.Apply();
		return cubemap;
	}
}
