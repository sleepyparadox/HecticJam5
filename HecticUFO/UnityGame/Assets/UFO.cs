using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityTools_4_6;

namespace HecticUFO
{
    public class UFO : UnityObject
    {
        public Vector3 MouseTarget;
        public UFOCamera Camera;
        public float DefaultSpeed = 15;
        public const float CollectRadius = 2f;
        Collider GroundCollider;
        public Transform CollectDest;
        public float CollectVelocity = 10f;
        public float ShootVelocity = 40f;
        public List<Prop> Collecting = new List<Prop>();
        public List<Prop> Collected = new List<Prop>();
        public UnityEngine.GameObject Mesh;
        private Vector3 MeshStartPos;
        Vector3 WhobbleAmount;
        int CollectCountMax = 25;
        List<GameObject> HPParticles;
        ParticleSystem FireEffect;
        TinyCoro HurtShake;

        Brush Brush;

        IEnumerator DoHurt()
        {
            var elapsed = 0f;
            var dur = 0.5f;
            var amount = UnityEngine.Random.Range(0, 100) > 50 ? 1f : -1f;
            amount *= 1f;
            while(elapsed < dur)
            {
                var nt = elapsed / dur;
                Mesh.transform.localPosition = new Vector3(Mathf.Sin(nt * Mathf.PI) * amount, Mesh.transform.localPosition.y, Mesh.transform.localPosition.z);
                yield return null;
                elapsed += Time.deltaTime;
            }
            Mesh.transform.localPosition = new Vector3(0, Mesh.transform.localPosition.y, Mesh.transform.localPosition.z);
        }

        public UFO()
            : base(Assets.Prefabs.UFOPrefab)
        {
            Beam = new UFOBeam();
            //Beam.Parent = this;
            Brush = Brush.UFOAbduct;
            GroundCollider = GameObject.Find("GroundBounce").GetComponent<Collider>();
            Camera = new UFOCamera();
            Camera.Parent = this;

            UnityUpdate += HandleInput;
            UnityDrawGizmos += (me) =>
            {
                Gizmos.color = Input.GetMouseButton(0) ? Color.red : Color.white;
                Gizmos.DrawWireSphere(MouseTarget, CollectRadius);
            };
            UnityGUI += (me) =>
            {
                if(Brush != HecticUFO.Brush.UFOAbduct)
                {
                    GUI.color = Color.red;
                    GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "Editing " + Brush + " layer");
                }
            };

            //Whobble
            Mesh = FindChild("Mesh");
            MeshStartPos = Mesh.transform.localPosition;
            WhobbleAmount = Vector3.up * 0.25f;

            HPParticles = new List<GameObject>();
            for (var i = 0; i < Mesh.transform.childCount - 3; ++i)
                HPParticles.Add(Mesh.transform.GetChild(i).gameObject);
            _hp = HPParticles.Count;
            FireEffect = Mesh.transform.FindChild("Fire").GetComponent<ParticleSystem>();
            FireEffect.enableEmission = false;
            SmokeEffect = Mesh.transform.FindChild("Smoke").GetComponent<ParticleSystem>();
            SmokeEffect.enableEmission = false;
            

            TinyCoro.SpawnNext(() => Shadow.Create(Mesh));

            //TODO, not whobbly dest
            CollectDest = Mesh.transform.FindChild("BeamGlow");
        }

        float CargoModifier = 1f;
        private UFOBeam Beam;

        int _hp = 7;
        private ParticleSystem SmokeEffect;
        public int Health
        {
            get
            {
                return _hp;
            }
            set
            {
                _hp = value;
                for (var i = 0; i < HPParticles.Count; ++i)
                    HPParticles[i].SetActive(_hp > i);
                
                if (HurtShake != null)
                    HurtShake.Kill();

                if(_hp > 0)
                {
                    HurtShake = TinyCoro.SpawnNext(DoHurt);
                }
                else
                {
                    UnityUpdate = null;
                    UnityUpdate += (me) => 
                    {
                        Transform.localRotation = Quaternion.Slerp(Transform.localRotation, Quaternion.identity, 6 * Time.deltaTime);
                        Camera.WorldPosition = new Vector3(Mesh.transform.position.x, 0, Mesh.transform.position.z);
                    };
                    Beam.SetActive(false);
                    var rigid = Mesh.AddComponent<Rigidbody>();
                    rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;

                    var rand = new Vector3(0.25f, UnityEngine.Random.Range(-1f, 1f), 0f);
                    rand.Normalize();

                    rigid.velocity += rand * 10f;
                    
                    FireEffect.enableEmission = true;
                    FireEffect.Play();
                    SmokeEffect.enableEmission = true;
                    SmokeEffect.Play();
                }
            }
        }
        void HandleInput(UnityObject me)
        {
            foreach (var prop in Collected)
                prop.LastAbductedAt = Time.time;
            foreach (var prop in Collecting)
                prop.LastAbductedAt = Time.time;

            Beam.SetActive(Input.GetMouseButton(0));
            Beam.Weak = Collecting.Count + Collected.Count >= CollectCountMax;

            var targetCargoModifer = 1 - ((Collecting.Count + Collected.Count) / (float)CollectCountMax);
            CargoModifier = Mathf.Lerp(CargoModifier, targetCargoModifer, Time.deltaTime);
            //Debug.Log((Collecting.Count + Collected.Count) + " of " + CollectCountMax + ", " + Collecting.Count + " collecting, " + Collected.Count + " collected");

            var modifiedSpeed = DefaultSpeed * CargoModifier;
            WorldPosition += Vector3.right * Input.GetAxis("Horizontal") * modifiedSpeed * Time.deltaTime;
            WorldPosition += Vector3.forward * Input.GetAxis("Vertical") * modifiedSpeed * Time.deltaTime;

            var heightWithWeight = MeshStartPos * (0.35f + (0.65f * CargoModifier));
            //Debug.Log("Cargo Mod " + CargoModifier + ", Height " + heightWithWeight);
            var whobble = Mathf.Sin(Time.time * 10f) * WhobbleAmount * CargoModifier;
            var oldX = Vector3.right * Mesh.transform.localPosition.x;
            Mesh.transform.localPosition = heightWithWeight + whobble + oldX;

            var mouseRay = Camera.UnityCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit mouseHit;
            if (GroundCollider.Raycast(mouseRay, out mouseHit, 1000))
            {
                MouseTarget = mouseHit.point;
                Debug.DrawLine(MouseTarget, MouseTarget + Vector3.up, Color.red);
                Camera.WorldPosition = Vector3.Lerp(WorldPosition, mouseHit.point, 0.25f);
            }

            const float mouseRadius = 10f;
            var mouseDir = (MouseTarget - WorldPosition);
            if(mouseDir.sqrMagnitude > (mouseRadius * mouseRadius))
            {
                MouseTarget = WorldPosition + (mouseDir.normalized * mouseRadius);
            }

            if (Input.GetKeyUp(KeyCode.X))
            {
                Brush = (Brush)(((int)Brush) + 1);
                HecticUFOGame.S.Map.BrushChanged(Brush);
            }
            if (Input.GetKeyUp(KeyCode.Z))
            {
                Brush = (Brush)(((int)Brush) - 1);
                HecticUFOGame.S.Map.BrushChanged(Brush);
            }

            if(Input.GetKeyUp(KeyCode.Backspace))
            {
                PlayerPrefs.SetInt("x", (int)WorldPosition.x);
                PlayerPrefs.SetInt("z", (int)WorldPosition.z);
                PlayerPrefs.Save();
                HecticUFOGame.S.Dispose();
                foreach (var coro in TinyCoro.AllCoroutines)
                    coro.Kill();
                Application.LoadLevel(0);
                return;
            }

            if(PlayerPrefs.HasKey("x") && PlayerPrefs.HasKey("z"))
            {
                WorldPosition = new Vector3(PlayerPrefs.GetInt("x"), 0, PlayerPrefs.GetInt("z"));
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
                return;
            }

            if(Brush != HecticUFO.Brush.UFOAbduct)
            {
                if(Input.GetMouseButton(0))
                    HecticUFOGame.S.Map.Apply(MouseTarget, Brush);
                else if(Input.GetMouseButton(1))
                    HecticUFOGame.S.Map.Clear(MouseTarget, Brush);
                return;
            }

            if(Input.GetMouseButton(0))
            {
                MusicAudio.S.Play(MusicAudio.S.UFOSuck, Mesh.transform.position, AudioStackRule.Singleton);
                var canCollect = (Collected.Count + Collecting.Count) < CollectCountMax;
                var rays = new List<Ray>();
                for (float x = -CollectRadius; x <= CollectRadius; x += CollectRadius / 5f)
                {
                    for (float z = -CollectRadius; z <= CollectRadius; z += CollectRadius / 5f)
                    {
                        if ((x * x) + (z * z) > (CollectRadius * CollectRadius))
                            continue;
                        var diff = new Vector3(MouseTarget.x + x, 0f, MouseTarget.z + z) - CollectDest.position;
                        var ray = new Ray(CollectDest.position, diff.normalized);
                        rays.Add(ray);


                        //Debug.DrawLine(ray.origin, ray.origin + diff, canCollect ? Color.green : Color.Lerp(Color.green, Color.black, 0.5f));
                    }
                }
                
                foreach(var prop in HecticUFOGame.S.Props)
                {
                    if (Collecting.Contains(prop)
                        || Collected.Contains(prop))
                        continue;

                    if (Collecting.Count + Collected.Count >= CollectCountMax)
                        break;

                    var collider = prop.GameObject.GetComponent<Collider>();
                    var rigidbody = prop.GameObject.GetComponent<Rigidbody>();

                    RaycastHit hit;
                    if(rays.Any((r) => collider.Raycast(r, out hit, 10000)))
                    {
                        var temp = prop;
                        Collecting.Add(temp);
                        TinyCoro.SpawnNext(() => PreformAbduct(temp));
                    }
                }
            }
            else
            {
                if(Input.GetMouseButtonDown(1)
                    && Collected.Any())
                {
                    TinyCoro.SpawnNext(PreformSpew);
                }
            }
        }

        IEnumerator PreformSpew()
        {
            var slowFireRate = 0.5f;
            var fastFireRate = 0.1f;
            var deltaFireRateMultiplier = 0.75f;

            var fireRate = slowFireRate;
            var elapsed = slowFireRate;
            while(Input.GetMouseButton(1)
                && Collected.Count > 0)
            {
                if(elapsed >= fireRate)
                {
                    var projectile = Collected[0];
                    Collected.RemoveAt(0);

                    MusicAudio.S.Play(MusicAudio.S.UFOBlow, Mesh.transform.position, AudioStackRule.OneShot);

                    //projectile.GameObject.layer = UnityEngine.Random.Range(0, 100) <= 50 ? Layers.PropStuck : Layers.PropBounce;
                    projectile.WillBounce();
                    projectile.LastAbductedAt = Time.time;

                    projectile.Transform.position = CollectDest.position;
                    projectile.SetActive(true);
                    var rigidbody = projectile.GameObject.GetComponent<Rigidbody>();
                    rigidbody.velocity = (MouseTarget - projectile.Transform.position).normalized * ShootVelocity;

                    fireRate = Mathf.Max(fastFireRate, fireRate * deltaFireRateMultiplier);
                    elapsed = 0f; //Reset
                }
                
                yield return null;
                elapsed += Time.deltaTime;
            }
        }

        IEnumerator PreformAbduct(Prop prop)
        {
            prop.WillBounce();
            prop.RestoreDrag();
            prop.Rigid.useGravity = false;
            var origonalScale = prop.Rigid.transform.localScale;
            var scale = 1f;
            var shrinkAt = 3f;
            var collectAtDist = 1f;
            while(Input.GetMouseButton(0)
                && Collected.Count < CollectCountMax)
            {
                var dist = CollectDest.position - prop.Transform.position;
                var distMag = dist.magnitude;
                
                //Collect
                if (distMag < collectAtDist)
                {
                    prop.Rigid.useGravity = true;
                    prop.Rigid.transform.localScale = origonalScale;
                    Collecting.Remove(prop);
                    prop.SetActive(false);
                    Collected.Add(prop);
                    yield break;
                }

                prop.Rigid.velocity = dist.normalized * CollectVelocity;

                scale = Math.Min(1f, distMag / shrinkAt);
                prop.Transform.localScale = origonalScale * scale;

                prop.LastAbductedAt = Time.time;

                yield return null;
            }

            //Dropped
            prop.Rigid.useGravity = true;

            //Pop back to origonal sice
            var popElapsed = 0f;
            var popDur = (1f - scale) / 2f;
            while(popElapsed < popDur)
            {
                var popScale = Mathf.Lerp(scale, 1, popElapsed / popDur);
                prop.Rigid.transform.localScale = origonalScale * popScale;
                yield return null;
                popElapsed += Time.deltaTime;
            }
            prop.Rigid.transform.localScale = origonalScale;
            Collecting.Remove(prop);
        }
    }
}
