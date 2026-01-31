#!/bin/bash

# -----------------------------
# Configuration
# -----------------------------
URLS=(
  "http://localhost:5287"
  "http://localhost:5287/hardware/tree"
  "http://localhost:5287/servers/list"
  "http://localhost:5287/resources/hardware/proxmox-node01"
)

RESOLUTION="1366,768"  # width,height
OUTPUT_DIR="./webui_screenshots"
GIF_OUTPUT="webui_screenshots/output.gif"
DELAY=200  # delay between frames in GIF (ms)

# -----------------------------
# Prepare output folder
# -----------------------------
mkdir -p "$OUTPUT_DIR"

# -----------------------------
# Capture screenshots
# -----------------------------
echo "Capturing screenshots..."
for URL in "${URLS[@]}"; do
  # sanitize filename
  FILENAME=$(echo "$URL" | sed 's~http[s]*://~~; s~/~_~g').png
  echo " - $URL -> $FILENAME"
  # headless Chrome screenshot
  /Applications/Google\ Chrome.app/Contents/MacOS/Google\ Chrome \
    --headless \
    --disable-gpu \
    --window-size=$RESOLUTION \
    --screenshot="$OUTPUT_DIR/$FILENAME" \
    "$URL"
done

# -----------------------------
# Convert to GIF using ImageMagick
# -----------------------------
echo "Creating GIF..."
convert -delay $DELAY -loop 0 "$OUTPUT_DIR"/*.png "$GIF_OUTPUT"

echo "Done! GIF saved to $GIF_OUTPUT"
