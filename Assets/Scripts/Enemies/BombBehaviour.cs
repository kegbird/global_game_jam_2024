using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBehaviour : MonoBehaviour, IKnockable
{
    public GameManager _game_manager;
    public Animator _bomb_animator;
    private Rigidbody2D _rigidbody;
    [SerializeField]
    private AudioSource _audio_source;
    public bool _spawned;

    private void Awake()
    {
        _spawned = true;
        _bomb_animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _game_manager = GameManager.instance;
    }

    public IEnumerator Boom()
    {
        if(_spawned)
        {
            _spawned = false;
        }
        else
        {
            _audio_source.Play();
            Vector3Int grid_position = _game_manager.GetGridPosition(transform.position);
            List<Vector3Int> grid_positions_in_bomb_range = _game_manager.GetBombRangePositions(grid_position);

            List<GameObject> enemies = _game_manager._enemies;
            Vector3Int player_grid_position = _game_manager.GetPlayerGridPosition();

            if (grid_positions_in_bomb_range.Contains(player_grid_position))
            {
                _game_manager.DamagePlayer(3);
            }

            for (int i = 0; i < enemies.Count; i++)
            {
                Vector3Int enemy_grid_position = _game_manager.GetGridPosition(enemies[i].transform.position);
                if (grid_positions_in_bomb_range.Contains(enemy_grid_position) && enemies[i].GetComponent<IEnemy>().IsAlive())
                {
                    enemies[i].GetComponent<IEnemy>().Kill();
                }
            }
            _bomb_animator.SetTrigger("explode");
        }

        yield return null;
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }

    public IEnumerator Knockback()
    {
        Vector3Int player_grid_position = _game_manager.GetPlayerGridPosition();
        Vector3 player_world_position = _game_manager.GetWorldPositionFromGridPosition(player_grid_position);
        Vector3 knockback_vector = transform.position - player_world_position;
        knockback_vector.Normalize();
        Vector3 target_position = transform.position + knockback_vector;
        while (Vector2.Distance(transform.position, target_position) > 0.001f)
        {
            _rigidbody.MovePosition(Vector2.MoveTowards(transform.position, target_position, 5f * Time.fixedDeltaTime));
            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }

}
