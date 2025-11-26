using Godot;
using System;

public partial class GoL : Node
{
	[Export] public int pixelWidth = 192;
	[Export] public int pixelHeight = 108;

	private Image img;
	private ImageTexture imgTexture;
	[Export] public TextureRect textRect;

	public int PixelWidth
	{
		get => pixelWidth;
		set => pixelWidth = value;
	}

	public int PixelHeight
	{
		get => pixelHeight;
		set => pixelHeight = value;
	}
	
	//instance
	
	//taille
	//img.create(width, height, false, Image.FORMAT_RGBA8)
	
	//colorier les pixels
	//img.set_pixel(5, 10, Color(1, 0, 0, 1)) # Rouge pur
	
	//créer une image texture
	//var texture = ImageTexture.create_from_image(img)
	
	//assigner à textureRect
	//Dans la fonction _ready() ou une autre fonction appropriée
	//$TextureRect.texture = texture

	/*Si vous souhaitez modifier les pixels pendant l'exécution du jeu, 
	vous devrez mettre à jour la texture. Vous pouvez le faire en appelant texture.create_from_image(img)
	 à nouveau et en réassignant la texture au TextureRect, ou en utilisant texture.update(img) 
	si la texture existe déjà.*/

	public override void _Ready()
	{
		base._Ready();
		img = Image.CreateEmpty(PixelWidth, PixelHeight, true, Image.Format.Rgba8);
		img.Fill(new Color(0, 0, 0));
		Random rdm = new Random();
		//colorier les pixels
		for (int i = 0; i < PixelWidth; i++)
		{
			for (int j = 0; j < PixelHeight; j++)
			{
				int temp = rdm.Next(0, 2);
				img.SetPixel(i, j, new Color(temp, temp, temp));
			}
		}

		imgTexture = ImageTexture.CreateFromImage(img);
		textRect.Texture = imgTexture;
		textRect.Size = new Vector2(pixelWidth, pixelHeight);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (Input.IsActionPressed("Quitter"))
		{
			GetTree().Quit();
		}
		Random rdm = new Random();
		int temp = 0;
		for (int i = 0; i < PixelWidth; i++)
		{
			for (int j = 0; j < PixelHeight; j++)
			{
				if (img.GetPixel(i, j).R == 0)
				{
					temp = 1;
				}
				else
				{
					temp = 0;
				}
				img.SetPixel(i, j, new Color(temp, temp, temp));
			}
		}
		textRect.Texture = ImageTexture.CreateFromImage(img);
		System.Threading.Thread.Sleep(100);
	}
}
