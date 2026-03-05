using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RBotTerminal : MonoBehaviour
{
    public RBot bot;
    public TMP_Text code;
    public float execInterval;
    public float moveSecondsPerTile;

    Coroutine execCoro;
    Vector2 direction=Vector2.up;

    public void RunCode() {
        StopRunning();
        List<(string, int)> cmds=MiniInterpreter.Parse(code.text);
        execCoro=bot.StartCoroutine(RunCodeCoro(cmds));
        bot.interactor=null;
        gameObject.SetActive(false);
    }
    public void StopRunning() {
        if (execCoro != null) {
            bot.StopCoroutine(execCoro);
            execCoro=null;
        }
    }
    struct LoopFrame
    {
        public int startIndex;
        public int remaining;
    }

    public IEnumerator RunCodeCoro(List<(string cmd, int value)> cmds)
    {
        Stack<LoopFrame> loopStack = new Stack<LoopFrame>();

        for (int i = 0; i < cmds.Count; i++)
        {
            var (cmd, n) = cmds[i];

            switch (cmd)
            {
                case "left":
                    CmdLeft(n);
                    yield return new WaitForSeconds(execInterval);
                    break;

                case "right":
                    CmdRight(n);
                    yield return new WaitForSeconds(execInterval);
                    break;

                case "move":
                    CmdMove(n);
                    yield return new WaitForSeconds(n*execInterval);
                    break;

                case "shoot":
                    CmdShoot(n);
                    yield return new WaitForSeconds(n*execInterval);
                    break;

                case "pickup":
                    CmdPickup(n);
                    yield return new WaitForSeconds(execInterval);
                    break;

                case "drop":
                    CmdDrop(n);
                    yield return new WaitForSeconds(execInterval);
                    break;

                case "use":
                    CmdUse(n);
                    yield return new WaitForSeconds(execInterval);
                    break;

                case "loop":
                    if (n > 0)
                    {
                        loopStack.Push(new LoopFrame
                        {
                            startIndex = i,
                            remaining = n
                        });
                    }
                    break;

                case "endloop":
                    if (loopStack.Count > 0)
                    {
                        var top = loopStack.Pop();
                        top.remaining--;

                        if (top.remaining > 0)
                        {
                            loopStack.Push(top);
                            i = top.startIndex; 
                        }
                    }
                    break;
            }
            yield return 0;
        }
        execCoro=null;
    }
    public static Vector2 Rotate(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);

        return new Vector2(
            v.x * cos - v.y * sin,
            v.x * sin + v.y * cos
        );
    }
    void CmdLeft(int n)
    {
        float baseAngle=45;
        n%=8;
        float angle=baseAngle*n;
        bot.spr.transform.localRotation*=Quaternion.Euler(new Vector3(0,0,angle));
        direction=Rotate(direction, angle);
    }
    void CmdRight(int n)
    {
        float baseAngle=45;
        n%=8;
        float angle=-baseAngle*n;
        bot.spr.transform.localRotation*=Quaternion.Euler(new Vector3(0,0,angle));
        direction=Rotate(direction, angle);
    }
    void CmdMove(int n)
    {
        bot.StartCoroutine(MoveCoro(n));
    }
    void CmdShoot(int n) { }
    void CmdPickup(int n) {
        // Check to see if we're on top of an item that can be held
        RaycastHit2D[] _maybeRaycastResults = new RaycastHit2D[15];
        int numObjectsFound = bot.rb.Cast(Vector2.zero, _maybeRaycastResults);
        for (int i = 0; i < numObjectsFound && i < _maybeRaycastResults.Length; i++) {
            RaycastHit2D result = _maybeRaycastResults[i];
            Tile tileHit = result.transform.GetComponent<Tile>();
            if (tileHit == null)
                continue;
            if (tileHit.hasTag(TileTags.CanBeHeld)) {
                tileHit.pickUp(bot);
            }
        }
    }
    void CmdDrop(int n)
    {
        if(bot.tileWereHolding!=null)
            bot.tileWereHolding.dropped(bot);
    }
    void CmdUse(int n)
    {
        bot.StartCoroutine(UseCoro(n));
    }
    IEnumerator UseCoro(int n) {
        WaitForSeconds wait=new WaitForSeconds(execInterval);
        for(int i = 0; i < n; ++i) {
            if (bot.tileWereHolding != null) {
                bot.tileWereHolding.useAsItem(bot);
            }
            yield return wait;
        }
    }
    IEnumerator MoveCoro(int n) {
        float time=moveSecondsPerTile*n;
        float dist=n*Tile.TILE_SIZE;
        float spd=dist/time;
        bot.rb.linearVelocity=direction*spd;
        yield return new WaitForSeconds(time);
        bot.rb.linearVelocity=Vector2.zero;
    }
}