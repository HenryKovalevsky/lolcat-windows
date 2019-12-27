namespace Lolcat

open System

// https://krazydad.com/tutorials/makecolors.php
module Colorize =
  type Settings =
    { Frequency: float
      PhaseRed: float
      PhaseGreen: float
      PhaseBlue: float
      PhaseOffset: float
      Center: float 
      Width: float }

  let defaultSettings =
    { Frequency = 2. * Math.PI / 180.
      PhaseRed = 2.
      PhaseGreen = 0.
      PhaseBlue = 4.
      PhaseOffset = 0.5
      Center = 184. 
      Width = 94. }

  let private seed = Random().Next(255)

  // Math.Sin(freq * i + phase + offset) * width + center
  let private calculateColor (settings : Settings) i =
    let v = float (seed + i)
    let red = int (Math.Sin(settings.Frequency * v + settings.PhaseRed + settings.PhaseOffset) * settings.Width + settings.Center)
    let green = int (Math.Sin(settings.Frequency * v + settings.PhaseGreen + settings.PhaseOffset) * settings.Width + settings.Center)
    let blue = int (Math.Sin(settings.Frequency * v + settings.PhaseBlue + settings.PhaseOffset) * settings.Width + settings.Center)
    (red, green, blue)

  let private buildColored ch (r, g, b) =
    sprintf "\u001b[38;2;%i;%i;%im%c" r g b ch

  let colorizeWithSettings (settings : Settings) (str : string) =
    let flip f x y = f y x
    str
    |> Seq.mapi (fun i ch -> (calculateColor settings i) |> buildColored ch)
    |> String.concat String.Empty
    |> flip (+)"\u001b[0m" // Set color back to default.

  let colorize = colorizeWithSettings defaultSettings
  