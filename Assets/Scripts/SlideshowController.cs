using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlideshowController : MonoBehaviour
{
    public RawImage slideshowImageDisplay;
    public Slider speedSlider;

    public Texture[] slideshowImages; // Array of textures for slideshow images
    private float slideshowSpeed = 2.0f; // Default speed
    private int currentImageIndex = 0;
    private float timer = 0.0f;

    public GoogleDriveImageImporter img;

    public TextMeshProUGUI pointer;

    private void Start()
    {
        // Initialize slideshow
        if (slideshowImages != null && slideshowImages.Length > 0)
        {
            SetImageWithAspectRatio(slideshowImages[0]);
        }

        // Set slider default value
        speedSlider.value = slideshowSpeed;
    }

    public void SetSlideshowImages(Texture[] images)
    {
        slideshowImages = images;
        if (slideshowImages.Length > 0)
        {
            SetImageWithAspectRatio(slideshowImages[0]);
        }
    }

    public void ClearSlides()
    {
        for (int i = 0; i < slideshowImages.Length; i++)
        {
            slideshowImages[i] = null;
        }

        slideshowImageDisplay.texture = null;
    }

    private void Update()
    {
        if(OVRInput.GetDown(OVRInput.Button.One))
        {
            speedSlider.value += 1;
            pointer.text = "A button was pressed";
        }

        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            speedSlider.value -= 1;
            pointer.text = "B button was pressed";
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ClearSlides();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            img.UploadImages();
            pointer.text = "A button was pressed";

        }




        // Update slideshow timer
        timer += Time.deltaTime;

        slideshowSpeed = speedSlider.value;

        // Change image if timer exceeds slideshowSpeed
        if (timer >= (1.0f / slideshowSpeed))
        {
            timer = 0.0f;
            ShowNextImage();
        }
    }

    private void ShowNextImage()
    {
        // Increment image index
        currentImageIndex = (currentImageIndex + 1) % slideshowImages.Length;
        // Display next image
        SetImageWithAspectRatio(slideshowImages[currentImageIndex]);
    }

    public void OnSpeedSliderChanged()
    {
        // Update slideshow speed based on slider value
    }

    // Method to add new images to the slideshow
    public void AddImagesToSlideshow(Texture[] newImages)
    {
        // Combine existing images with new images
        int currentLength = slideshowImages != null ? slideshowImages.Length : 0;
        int newLength = newImages != null ? newImages.Length : 0;

        Texture[] combinedImages = new Texture[currentLength + newLength];

        if (currentLength > 0)
        {
            slideshowImages.CopyTo(combinedImages, 0);
        }

        if (newLength > 0)
        {
            newImages.CopyTo(combinedImages, currentLength);
        }

        slideshowImages = combinedImages;

        // If slideshow is empty, display the first image
        if (slideshowImages.Length > 0)
        {
            SetImageWithAspectRatio(slideshowImages[0]);
        }
    }

    private void SetImageWithAspectRatio(Texture texture)
    {
        if (texture == null) return;

        slideshowImageDisplay.texture = texture;

        // Get the aspect ratio of the texture
        float aspectRatio = (float)texture.width / texture.height;

        // Get the parent RectTransform to ensure it fits within its bounds
        RectTransform parentRt = slideshowImageDisplay.transform.parent.GetComponent<RectTransform>();
        float parentWidth = parentRt.rect.width;
        float parentHeight = parentRt.rect.height;

        // Adjust the RawImage size
        RectTransform rt = slideshowImageDisplay.GetComponent<RectTransform>();
        if (aspectRatio > 1) // Landscape
        {
            float width = parentWidth;
            float height = width / aspectRatio;
            if (height > parentHeight)
            {
                height = parentHeight;
                width = height * aspectRatio;
            }
            rt.sizeDelta = new Vector2(width, height);
        }
        else // Portrait or Square
        {
            float height = parentHeight;
            float width = height * aspectRatio;
            if (width > parentWidth)
            {
                width = parentWidth;
                height = width / aspectRatio;
            }
            rt.sizeDelta = new Vector2(width, height);
        }
    }
}
