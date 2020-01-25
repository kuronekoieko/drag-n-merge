using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScoreNumTextManager : MonoBehaviour
{
    [SerializeField] ScoreNumTextController scoreNumTextPrefab;
    ScoreNumTextController[] scoreNumTextControllers;

    public static ScoreNumTextManager i;
    void Awake()
    {
        i = this;
    }

    public void OnStart()
    {
        TextGenerator();
    }


    public void OnUpdate()
    {

    }

    void TextGenerator()
    {
        int blockNum = Values.BOARD_LENGTH_X * Values.BOARD_LENGTH_Y;
        scoreNumTextControllers = new ScoreNumTextController[blockNum];

        int i = 0;
        for (int iy = 0; iy < Values.BOARD_LENGTH_Y; iy++)
        {
            for (int ix = 0; ix < Values.BOARD_LENGTH_X; ix++)
            {

                scoreNumTextControllers[i] = Instantiate(
                    scoreNumTextPrefab,
                    Vector3.zero,
                    Quaternion.identity,
                    transform);

                scoreNumTextControllers[i].OnStart(
                    name: "text_" + i,
                    indexX: ix,
                    indexY: iy
                    );

                i++;
            }
        }
    }

    public void ShowScoreNum(int scoreNum, Vector3 pos)
    {
        ScoreNumTextController score = scoreNumTextControllers
            .Where(s => s.isWaiting)
            .FirstOrDefault();

        if (score == null) { return; }
        score.ShowScoreNum(scoreNum: scoreNum, pos: pos);
    }
}
