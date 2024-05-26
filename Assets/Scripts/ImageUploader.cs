using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class ImageUploader : MonoBehaviour
{
    private SlideshowController slideshowController;

    private void Start()
    {
        // Find SlideshowController in the scene
        slideshowController = FindObjectOfType<SlideshowController>();
    }

    // Method to open file picker and load selected image
    public void OpenFilePicker()
    {
        StartCoroutine(PickImageCoroutine());
    }

    private IEnumerator PickImageCoroutine()
    {
        // Open file picker
        NativeFilePicker.PickFile((path) =>
        {
            if (!string.IsNullOrEmpty(path))
            {
                LoadImageFromFile(path);
            }
        });

        yield return null;
    }

    // Load image from file path and pass it to SlideshowController
    private void LoadImageFromFile(string path)
    {
        // Read image file from path
        byte[] imageData = File.ReadAllBytes(path);

        // Create texture from image data
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageData);

        // Add the new image to the slideshow in SlideshowController
        if (slideshowController != null)
        {
            slideshowController.AddImagesToSlideshow(new Texture[] { texture });
        }
    }

    public void end()
    {
        Application.Quit();
    }
}
