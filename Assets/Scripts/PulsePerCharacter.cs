
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PulsePerCharacter : MonoBehaviour
{
 public float pulseScale = 1.3f;
    public float pulseDuration = 0.25f;
    public float delayBetweenLetters = 0.05f;
    public bool loop = true;

    TMP_Text textMesh;
    TMP_TextInfo textInfo;
    Vector3[][] originalVertices;
      public AudioManager audioManager;
    public AudioClip audioClip;
  

    void Awake()
    {
        textMesh = GetComponent<TMP_Text>();
    }

    void OnEnable()
    {
        StartCoroutine(PopLetters());

    }

    IEnumerator PopLetters()
    {
        yield return new WaitForSeconds(0.1f); // tiny delay so TMP generates mesh
audioManager.PlayAudio(audioClip);
        textMesh.ForceMeshUpdate();
        textInfo = textMesh.textInfo;

        // Store original vertex data
        originalVertices = new Vector3[textInfo.meshInfo.Length][];
        for (int i = 0; i < originalVertices.Length; i++)
        {
            originalVertices[i] = new Vector3[textInfo.meshInfo[i].vertices.Length];
            System.Array.Copy(textInfo.meshInfo[i].vertices, originalVertices[i], originalVertices[i].Length);
        }

        int totalChars = textInfo.characterCount;

        do
        {
            for (int i = 0; i < totalChars; i++)
            {
                if (!textInfo.characterInfo[i].isVisible)
                    continue;

                yield return StartCoroutine(PulseCharacter(i));
                yield return new WaitForSeconds(delayBetweenLetters);
            }
        }
        while (loop);
    }

    IEnumerator PulseCharacter(int index)
    {
        textMesh.ForceMeshUpdate();
        textInfo = textMesh.textInfo;

        var charInfo = textInfo.characterInfo[index];
        if (!charInfo.isVisible) yield break;

        int materialIndex = charInfo.materialReferenceIndex;
        int vertexIndex = charInfo.vertexIndex;

        Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
        Vector3[] original = originalVertices[materialIndex];

        float half = pulseDuration / 2f;
        float t = 0f;

        // Scale up
        while (t < half)
        {
            float scale = Mathf.Lerp(1f, pulseScale, t / half);
            ApplyScale(vertices, original, vertexIndex, scale);
            UpdateMesh(materialIndex);
            t += Time.deltaTime;
            yield return null;
        }

        // Ensure max
        ApplyScale(vertices, original, vertexIndex, pulseScale);
        UpdateMesh(materialIndex);
        t = 0f;

        // Scale back down
        while (t < half)
        {
            float scale = Mathf.Lerp(pulseScale, 1f, t / half);
            ApplyScale(vertices, original, vertexIndex, scale);
            UpdateMesh(materialIndex);
            t += Time.deltaTime;
            yield return null;
        }

        // Reset
        ApplyScale(vertices, original, vertexIndex, 1f);
        UpdateMesh(materialIndex);
    }

    void ApplyScale(Vector3[] verts, Vector3[] orig, int idx, float scale)
    {
        Vector3 mid = (orig[idx] + orig[idx + 1] + orig[idx + 2] + orig[idx + 3]) / 4f;

        for (int j = 0; j < 4; j++)
        {
            verts[idx + j] = mid + (orig[idx + j] - mid) * scale;
        }
    }

    void UpdateMesh(int index)
    {
        textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }
}
