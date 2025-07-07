using Com.LuisPedroFonseca.ProCamera2D;
using FlamingCore;
using UnityEngine;

public class BoomManager : MonoBehaviour
{
  private Collider[] col;

  [SerializeField]
  private LayerMask canBeBoomed;

  private Rigidbody rbTemp;

  private Combat combatTemp;

  private Breakable breakAbleObject;

  private void Start()
  {
	col = new Collider[10];
  }

  private void Update()
  {
  }

  public void Boom(Vector3 position, float radius, float force, int hurt, int team)
  {
	int num = 0;
	int num2 = 0;
	bool killedPlayer = false;
	ProCamera2DShake.Instance.Shake("KillShake");
	AudioManager.PlaySFXAtPosition("Explode", position);
	if (radius > 3.55f)
	{
	  EmitParticleAtPoint(position, GameManager.Instance.ParticleManager.BoomParticlePfb);
	}
	else
	{
	  EmitParticleAtPoint(position, GameManager.Instance.ParticleManager.BoomParticleSmallPfb);
	}
	int num3 = Physics.OverlapSphereNonAlloc(position, radius, col, canBeBoomed);
	for (int i = 0; i < num3; i++)
	{
	  if (!(col[i] != null))
	  {
		continue;
	  }
	  combatTemp = col[i].GetComponent<Combat>();
	  if (combatTemp != null && !combatTemp.ignoreExplosion)
	  {
		bool isDead = combatTemp.IsDead;
		combatTemp.Hurt(hurt, team, col[i].transform.position);
		num++;
		if (!isDead && combatTemp.IsDead)
		{
		  num2++;
		  if (combatTemp.IsHead())
		  {
			killedPlayer = true;
		  }
		}
	  }
	  else
	  {
		breakAbleObject = col[i].GetComponent<Breakable>();
		if (breakAbleObject != null)
		{
		  breakAbleObject.Hurt(hurt);
		}
	  }
	  rbTemp = col[i].GetComponent<Rigidbody>();
	  if (rbTemp != null)
	  {
		rbTemp.AddForce(force * (FCTool.Vector3YToZero((col[i].transform.position - position).normalized) + UnityEngine.Random.Range(1f, 4f) * Vector3.up));
		rbTemp.angularVelocity = UnityEngine.Random.insideUnitSphere * 600f;
	  }
	}
	DataManager.BombDamageContext context = default(DataManager.BombDamageContext);
	context.killCount = num2;
	context.hurtCount = num;
	context.killedPlayer = killedPlayer;
	DataManager.HandleBombDamage(context);
  }

  public void BoomWithTargets(Vector3 position, float radius, float force, int hurt, int team, Collider[] targets)
  {
    int hurtCount = 0;
    int killCount = 0;
    bool killedPlayer = false;

    ProCamera2DShake.Instance.Shake("KillShake");
    AudioManager.PlaySFXAtPosition("Explode", position);

    if (radius > 3.55f)
      EmitParticleAtPoint(position, GameManager.Instance.ParticleManager.BoomParticlePfb);
    else
      EmitParticleAtPoint(position, GameManager.Instance.ParticleManager.BoomParticleSmallPfb);

    foreach (var collider in targets)
    {
      if (collider == null)
        continue;

      Combat combat = collider.GetComponent<Combat>();
      if (combat != null && !combat.ignoreExplosion)
      {
        bool wasAlive = !combat.IsDead;
        combat.Hurt(hurt, team, collider.transform.position);
        hurtCount++;

        if (wasAlive && combat.IsDead)
        {
          killCount++;
          if (combat.IsHead())
            killedPlayer = true;
        }
      }
      else
      {
        Breakable breakable = collider.GetComponent<Breakable>();
        if (breakable != null)
          breakable.Hurt(hurt);
      }

      Rigidbody rb = collider.GetComponent<Rigidbody>();
      if (rb != null)
      {
        Vector3 dir = FCTool.Vector3YToZero((collider.transform.position - position).normalized);
        rb.AddForce(force * (dir + Random.Range(1f, 4f) * Vector3.up));
        rb.angularVelocity = Random.insideUnitSphere * 600f;
      }
    }

    DataManager.BombDamageContext context = default;
    context.hurtCount = hurtCount;
    context.killCount = killCount;
    context.killedPlayer = killedPlayer;
    DataManager.HandleBombDamage(context);
  }


  private void EmitParticleAtPoint(Vector3 _pos, ParticleSystem particle)
  {
	Object.Instantiate(particle).transform.position = _pos;
  }
}