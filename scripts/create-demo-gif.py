from pathlib import Path

from PIL import Image


ROOT = Path(__file__).resolve().parents[1]
SCREENSHOTS = ROOT / "docs" / "screenshots"
OUTPUT = SCREENSHOTS / "gradeflow-demo.gif"

SLIDES = [
    ("Login", "login.png"),
    ("Dashboard", "dashboard.png"),
    ("Avaliacoes", "avaliacoes.png"),
    ("Nova avaliacao", "nova-avaliacao.png"),
    ("Novo usuario", "novo-usuario.png"),
    ("Sobre", "sobre.png"),
]


def fit(image: Image.Image, size: tuple[int, int]) -> Image.Image:
    image = image.convert("RGB")
    image.thumbnail(size, Image.Resampling.LANCZOS)
    canvas = Image.new("RGB", size, "#050816")
    x = (size[0] - image.width) // 2
    y = (size[1] - image.height) // 2
    canvas.paste(image, (x, y))
    return canvas


def main() -> None:
    base_size = (960, 667)
    frames: list[Image.Image] = []
    durations: list[int] = []

    images = [(title, fit(Image.open(SCREENSHOTS / name), base_size)) for title, name in SLIDES]
    for index, (title, image) in enumerate(images):
        frames.append(image)
        durations.append(1100)

        if index == len(images) - 1:
            continue

        _, next_image = images[index + 1]
        for alpha in (0.25, 0.5, 0.75):
            frames.append(Image.blend(image, next_image, alpha))
            durations.append(90)

    frames[0].save(
        OUTPUT,
        save_all=True,
        append_images=frames[1:],
        duration=durations,
        loop=0,
        optimize=True,
    )


if __name__ == "__main__":
    main()
