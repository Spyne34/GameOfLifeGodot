using Godot;
using System;

public partial class GoL : Node
{
	[Export] public int pixelWidth = 192;
	[Export] public int pixelHeight = 108;
	[Export] private Label _speedLabel;

	private Image imgs;
	private ImageTexture imgTexture;
	private TextureRect textRects;
	//[Export] public TextureRect textRects;
	//[Export] public int nbGen = 1;
	[Export] public float speed = 1;
	// en seconde
	private double timer = 1;

	private Vector2[,] world;
	
	private bool paused = false;
	
	[Export] public int maxAge = 6;
	
	//plus le fade est proche de 1 plus la trainée est longue
	[Export] public float fade = 0.75f;
	
	[Export] public int numberToDie = 1;
	[Export] public int numberToStay = 2;
	[Export] public int numberToLive = 3;

	[Export] public int distanceVoisins = 1;

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

	public int WorldWidth
	{
		get => world.GetLength(0);
	}

	public int WorldHeight
	{
		get => world.GetLength(1);
	}

	public Vector2 World(int x, int y, float life = -1, float age = -1)
	{
		if (x < 0)
		{
			x = WorldWidth + x;
		}

		if (x >= WorldWidth)
		{
			x = x - WorldWidth;
		}

		if (y < 0)
		{
			y = WorldHeight + y;
		}

		if (y >= WorldHeight)
		{
			y = y - WorldHeight;
		}

		if (life >= 0)
		{
			world[x, y].X = life;
		}
		
		if (age >= 0)
		{
			world[x, y].Y = age;
		}
		
		return world[x, y];
	}

	public override void _Ready()
	{
		base._Ready();
		
		//background
		TextureRect backgroundText = new TextureRect();
		Image backgroundImg = Image.CreateEmpty(PixelWidth, PixelHeight, false, Image.Format.Rgba8);
		backgroundImg.Fill(Colors.MidnightBlue);
		backgroundText = new TextureRect();
		backgroundText.Texture = ImageTexture.CreateFromImage(backgroundImg);
		backgroundText.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
		backgroundText.TextureRepeat = CanvasItem.TextureRepeatEnum.Disabled;
		backgroundText.StretchMode = TextureRect.StretchModeEnum.Scale;
		backgroundText.Size = GetViewport().GetVisibleRect().Size;
		AddChild(backgroundText);

		UpdateSpeedLabel(speed);
		
		world = new Vector2[PixelWidth, PixelHeight];

		//textRects = new TextureRect();
		// textRects = new TextureRect[nbGen];
		// imgs = new Image[nbGen];
		// imgTexture = new ImageTexture[nbGen];
			
		// for (int k = 0; k < nbGen; k++)
		// {
		// 	//Créer les images et colorier les pixels
		// 	imgs[k] = Image.CreateEmpty(PixelWidth, PixelHeight, false, Image.Format.Rgba8);
		// 	imgs[k].Fill(new Color(0, 0, 0));
		// 	//imgs[k].Fill(new Color(0, 0, 0, 0.2f));
		// }
		
		imgs = Image.CreateEmpty(PixelWidth, PixelHeight, false, Image.Format.Rgba8);
		imgs.Fill(Colors.MidnightBlue);

		imgTexture = ImageTexture.CreateFromImage(imgs);
		textRects = new TextureRect
		{
			Texture = imgTexture,
			TextureFilter = CanvasItem.TextureFilterEnum.Nearest,
			StretchMode = TextureRect.StretchModeEnum.Scale,
			Size = GetViewport().GetVisibleRect().Size
		};
		
		AddChild(textRects);
		
		Random rdm = new Random();
		
		GD.Print("test0");
		for (int i = 0; i < PixelWidth; i++)
		{
			for (int j = 0; j < PixelHeight; j++)
			{
				//Set l'état des cellules aléatoirement
				int temp = rdm.Next(0, 2);
				World(i, j, temp, temp);
				if (temp == 1)
				{
					imgs.SetPixel(i, j, ColorFromAge((int)World(i, j).Y));
				}
				
				// for (int k = 0; k < nbGen; k++)
				// {
				// 	imgs[k].SetPixel(i, j, new Color(temp, temp, temp));
				// 	//imgs[k].SetPixel(i, j, new Color(temp, temp, temp,0.2f));
				// }
			}
		}
		
		// for (int k = nbGen - 1; k >= 0; k--)
		// {
		// 	//Créer les textures et les paramétrer
		// 	textRects[k] = new TextureRect();
		// 	imgTexture[k] = ImageTexture.CreateFromImage(imgs[k]);
		// 	textRects[k].Texture = imgTexture[k];
		// 	textRects[k].TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
		// 	textRects[k].TextureRepeat = CanvasItem.TextureRepeatEnum.Disabled;
		// 	textRects[k].StretchMode = TextureRect.StretchModeEnum.Scale;
		// 	textRects[k].Modulate = new Color(1, 1, 1, GetAlpha(k));
		// 	textRects[k].Size = GetViewport().GetVisibleRect().Size;
		// 	AddChild(textRects[k]);
		// }

		//Texture rects
		//textRect.Size = new Vector2(pixelWidth, pixelHeight);
		GD.Print("test1");
		//GD.Print(WorldWidth, WorldHeight);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		
		//GD.Print("test2");
		if (Input.IsActionPressed("Quitter"))
		{
			GetTree().Quit();
		}

		if (Input.IsActionJustPressed("Pause"))
		{
			paused = !paused;
		}

		if (Input.IsActionJustPressed("Speed Up"))
		{
			speed *= 2;
			UpdateSpeedLabel(speed);
		}
		
		if (Input.IsActionJustPressed("Speed Down"))
		{
			speed /= 2;
			UpdateSpeedLabel(speed);
		}
		
		if (Input.IsActionPressed("Dessiner"))
		{
			Vector2 pixel = GetWorldPixel(GetViewport().GetMousePosition());
			int x = (int)pixel.X;
			int y = (int)pixel.Y;
			World(x, y, 1, 1);
			imgs.SetPixel(x, y, ColorFromAge((int)World(x, y).Y));
			imgTexture.Update(imgs);
		}
		
		if (Input.IsActionPressed("Effacer"))
		{
			Vector2 pixel = GetWorldPixel(GetViewport().GetMousePosition());
			int x = (int)pixel.X;
			int y = (int)pixel.Y;
			World(x, y, 0, 0);
			imgs.SetPixel(x, y, ColorFromAge((int)World(x, y).Y));
			imgTexture.Update(imgs);
		}

		
		if (paused) return;

		timer -= delta;
		if (timer > 0) return;
		timer = 1.0 / Math.Max(speed, 0.0001);

		
		NewGen();
		
		// for (int k = 1; k < nbGen; k++)
		// {
		// 	imgs[k].CopyFrom(imgs[k - 1]);
		// }
		
		for (int i = 0; i < PixelWidth; i++)
		{
			for (int j = 0; j < PixelHeight; j++)
			{
				Color c = imgs.GetPixel(i, j);
				c.A *= fade;
				imgs.SetPixel(i, j, c);
			}
		}
		
		for (int i = 0; i < WorldWidth; i++)
		{
			for (int j = 0; j < WorldHeight; j++)
			{
				if (World(i, j).X >= 1)
				{
					// float temp = World(i, j).Y;
					// Color color;
					// switch ((int)temp)
					// {
						//case 0: color = Colors.MidnightBlue; break;
						// case 1: color = Colors.DarkGreen; break;
						// case 1: color = Colors.Green; break;
						// case 2: color = Colors.GreenYellow; break;
						// case 3: color = Colors.Gold; break;
						// case 4: color = Colors.DarkOrange; break;
						// case 5: color = Colors.OrangeRed; break;
						// case 6: color = Colors.Red; break;
					// 	default: color = Colors.White; break;
					// }
					//
					// //color.A = 0.2f;
					// imgs[0].SetPixel(i, j, color);
					imgs.SetPixel(i, j, ColorFromAge((int)World(i, j).Y));
				}
			}
		}
		imgTexture.Update(imgs);
		
		// for (int k = 0; k < nbGen; k++)
		// {
		// 	imgTexture[k].Update(imgs[k]);
		// 	//textRects[k].Texture = imgTexture[k];
		// 	textRects[k].Modulate  = new Color(1, 1, 1, GetAlpha(k));
		// }
	}

	private void UpdateSpeedLabel(float speed)
	{
		_speedLabel.Text = $"{speed}x";
	}

	public void NewGen()
	{
		Vector2[,] newWorld = new Vector2[PixelWidth, PixelHeight];
		newWorld = world.Clone() as Vector2[,];
		for (int i = 0; i < WorldWidth; i++)
		{
			for (int j = 0; j < WorldHeight; j++)
			{
				int count = 0;
				for (int k = i - distanceVoisins; k <= i + distanceVoisins; k++)
				{
					for (int l = j - distanceVoisins; l <= j + distanceVoisins; l++)
					{
						if ( (k != i || l != j) && World(k, l).X >= 1)
						{
							count++;
						}
					}
				}

				if (count <= numberToDie)
				{
					newWorld[i, j].X = 0;
					newWorld[i, j].Y = 0;
				}
				else if (count <= numberToStay)
				{
					if (newWorld[i, j].X == 0)
					{
						newWorld[i, j].Y = 0;
					}
					else if (newWorld[i, j].Y < maxAge)
					{
						newWorld[i, j].Y += 1;
					}
				}
				else if (count <= numberToLive)
				{
					newWorld[i, j].X = 1;
					if (newWorld[i, j].Y < maxAge)
					{
						newWorld[i, j].Y += 1;
					}
				}
				else
				{
					newWorld[i, j].X = 0;
					newWorld[i, j].Y = 0;
				}
			}
		}
		world = newWorld.Clone() as Vector2[,];
	}

	// public float GetAlpha(int index, float f0 = 0.6f, float fMax = 0.1f, float max = 10f)
	// {
	// 	if (nbGen == 1)
	// 	{
	// 		return 1;
	// 	}
	// 	
	// 	if (index <= max)
	// 	{
	// 		return f0 - (f0 - fMax) / Mathf.Log(max + 1f) * Mathf.Log(index + 1f);
	// 	}
	// 	else
	// 	{
	// 		return fMax;
	// 	}
	// }
	
	// Color ColorFromAge(int age, int maxAge)
	// {
	// 	age -= 1;
	// 	maxAge -= 1;
	// 	// Clamp de sécurité
	// 	age = Mathf.Clamp(age, 0, maxAge);
	//
	// 	// Normalisation de l’âge entre 0 et 1
	// 	float t = age / (float)maxAge;
	//
	// 	/*
	// 		t = 0   → jeune cellule
	// 		t = 1   → vieille cellule
	// 	*/
	//
	// 	// Dégradé de couleur (du vivant vers le chaud)
	// 	// Color young = new Color(0.1f, 0.8f, 0.2f); // vert
	// 	Color young = Colors.DarkGreen;
	// 	// Color old   = new Color(1.0f, 0.3f, 0.1f); // rouge/orange
	// 	Color old   = Colors.Red;
	//
	// 	// Interpolation linéaire
	// 	Color color = young.Lerp(old, t);
	//
	// 	// Alpha toujours à 1 (le fondu est géré ailleurs)
	// 	color.A = 1f;
	//
	// 	return color;
	// }
	
	public Color ColorFromAge(float age)
	{
		if (age <= 0)
		{
			return Colors.Transparent;
		}

		
		Color[] palette = new Color[]
		{
			Colors.DarkGreen,
			Colors.Green,
			Colors.GreenYellow,
			Colors.Gold,
			Colors.DarkOrange,
			Colors.OrangeRed,
			Colors.Red
		};

		int steps = palette.Length - 1;

		float t = Mathf.Clamp((age - 1) / (maxAge - 1), 0f, 1f);

		
		float scaled = t * steps;
		int index = Mathf.FloorToInt(scaled);
		float localT = scaled - index;


		if (index >= steps)
		{
			return palette[steps];
		}

		return palette[index].Lerp(palette[index + 1], localT);
	}

	Vector2 GetWorldPixel(Vector2 mousePosition)
	{
		//GD.Print("mousePosition : " + mousePosition);
		
		Vector2 screenSize = GetViewport().GetVisibleRect().Size;
		mousePosition.X = mousePosition.X / screenSize.X;
		mousePosition.Y = mousePosition.Y / screenSize.Y;
		
		//GD.Print("normalisé : " + mousePosition);

		mousePosition.X *= pixelWidth;
		mousePosition.Y *= pixelHeight;
		
		//GD.Print("dans le World : " + mousePosition + "\n");
		return mousePosition;
	}
}
