using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    [Tooltip("Lista completa de PowerUpData (opcional, se usa para buscar por id)")]
    public PowerUpData[] allPowerUps;
    public GameConfig config;

    private PlayerModel _playerModel;
    private GameModel _gameModel;
    private Dictionary<string, Coroutine> _active = new Dictionary<string, Coroutine>();
    private IEventBus _bus;

    void Start()
    {
        _playerModel = GameInstaller.Instance.PlayerModel;
        _game_model_check();
        _gameModel = GameInstaller.Instance.GameModel;
        _bus = GameContainer.Resolve<IEventBus>();
        _bus.Subscribe<PowerUpCollectedEvent>(OnPowerupCollected);
    }

    void OnDestroy()
    {
        if (_bus != null) _bus.Unsubscribe<PowerUpCollectedEvent>(OnPowerupCollected);
    }

    private void OnPowerupCollected(PowerUpCollectedEvent evt)
    {
        // buscar PowerUpData por id en la lista (o recibir la referencia directa en el evento)
        var data = System.Array.Find(allPowerUps, p => p != null && p.id == evt.powerUpId);
        if (data == null)
        {
            Debug.LogWarning($"PowerupManager: no encontrado PowerUpData con id '{evt.powerUpId}'");
            return;
        }

        if (_active.ContainsKey(data.id))
        {
            // reiniciar timer
            StopCoroutine(_active[data.id]);
            _active[data.id] = StartCoroutine(RunEffect(data));
        }
        else
        {
            _active[data.id] = StartCoroutine(RunEffect(data));
        }
    }

    private IEnumerator RunEffect(PowerUpData data)
    {
        // Aplicar efecto usando Strategy (PowerUpEffectBase referenciado)
        if (data.effect != null)
        {
            data.effect.Apply(_playerModel, _gameModel);
        }
        else
        {
            // fallback: aplicar por tipo si no hay effect referenciado
            ApplyFallbackByType(data);
        }

        float duration = data.duration > 0f ? data.duration : config.powerupDefaultDuration;

        // usar WaitForSecondsRealtime para ignorar Time.timeScale (importante si SlowTime se usa)
        yield return new WaitForSecondsRealtime(duration);

        // eliminar efecto
        if (data.effect != null)
        {
            data.effect.Remove(_playerModel, _gameModel);
        }
        else
        {
            RemoveFallbackByType(data);
        }

        _active.Remove(data.id);
    }

    private void ApplyFallbackByType(PowerUpData data)
    {
        switch (data.type)
        {
            case PowerUpType.Shield:
                _playerModel.HasShield = true;
                break;
            case PowerUpType.DoubleJump:
                _playerModel.CanDoubleJump = true;
                break;
            case PowerUpType.DoublePoints:
                _playerModel.ScoreMultiplier = 2f;
                break;
            case PowerUpType.SlowTime:
                Time.timeScale = 0.5f;
                break;
        }
    }

    private void RemoveFallbackByType(PowerUpData data)
    {
        switch (data.type)
        {
            case PowerUpType.Shield:
                _playerModel.HasShield = false;
                break;
            case PowerUpType.DoubleJump:
                _playerModel.CanDoubleJump = false;
                break;
            case PowerUpType.DoublePoints:
                _playerModel.ScoreMultiplier = 1f;
                break;
            case PowerUpType.SlowTime:
                Time.timeScale = 1f;
                break;
        }
    }

    // pequeño chequeo para evitar NRE si GameInstaller no está listo
    private void _game_model_check()
    {
        if (GameInstaller.Instance == null)
        {
            Debug.LogError("PowerupManager: GameInstaller.Instance es null en Start(). Asegúrate de que GameInstaller está en la escena y se ejecuta antes.");
            return;
        }
    }
}
