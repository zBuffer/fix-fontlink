# Fix-FontLink

This program is written to fix the display of Chinese font on Windows 10/Windows 11 English.

## Problem

When displaying Simplified Chinese characters, Windows Font Linking by default falls back to Japanese font (Meiryo), 
then Traditional Chinese font (JhengHei), then Simplified Chinese font (YaHei).

Because the above fonts do not have a consistent size and weight, the display of Simplified Chinese characters is ugly 
when there is a different mix of fonts involved.

This problem was not present when Microsoft had license for the Arial Unicode font.

## Solution

This program sorts the Font Linking priority, optimized for the display of Simplified Chinese characters in the following order:

```
"MSYH.TTC", "MSJH.TTC", "MEIRYO.TTC", "MALGUN.TTF", "TAHOMA.TTF"
```

In other words, the priority looks like:

1. Simplified Chinese
2. Traditional Chinese
3. Japanese
4. Korean
5. Latin

If you would like to optimize it to display other languages instead, change the array in `Program.cs:25` and put the font corresponding 
to your language as the first element.

i.e.
```
"MSJH.TTC", "MSYH.TTC", "MEIRYO.TTC", "MALGUN.TTF", "TAHOMA.TTF"
```

This would optimize the display of Traditional Chinese characters.
