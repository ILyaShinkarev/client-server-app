export function getImageMetrics(img) {
    const rect = img.getBoundingClientRect();
    return {
        naturalWidth: img.naturalWidth,
        naturalHeight: img.naturalHeight,
        renderedWidth: rect.width,
        renderedHeight: rect.height
    };
}
