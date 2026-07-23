using UnityEngine;
using System.Collections;

public class GravityReverseItem : MonoBehaviour
{
    [SerializeField] private bool oneTimeUse = true;
    [SerializeField] private float respawnTime = 5f;

    private ParticleSystem ps;
    private AudioSource aud;
    private MeshRenderer meshRenderer;
    private Collider col;

    private void Start()
    {
        ps = GetComponentInChildren<ParticleSystem>();
        aud = GetComponent<AudioSource>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        col = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // もしプレイヤーのタグが設定されていない場合も、タグ検索で見つける
        PlayerMove2 player = null;

        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<PlayerMove2>();
        }
        else
        {
            // タグから検索して自動取得（ステージがPrefabでもOK）
            GameObject foundPlayer = GameObject.FindWithTag("Player");
            if (foundPlayer != null)
                player = foundPlayer.GetComponent<PlayerMove2>();
        }

        if (player == null) return;

        // 上下反転処理を実行
        player.ToggleUpsideDown();
        Debug.Log($"{player.name} が上下反転！");

        // エフェクト・音
        if (ps != null) ps.Play();
        if (aud != null) aud.Play();

        // 一度だけ or 復活あり
        if (oneTimeUse)
            StartCoroutine(Disappear());
        else
            StartCoroutine(Respawn());
    }

    private IEnumerator Disappear()
    {
        meshRenderer.enabled = false;
        col.enabled = false;
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }

    private IEnumerator Respawn()
    {
        meshRenderer.enabled = false;
        col.enabled = false;
        yield return new WaitForSeconds(respawnTime);
        meshRenderer.enabled = true;
        col.enabled = true;
    }
}
