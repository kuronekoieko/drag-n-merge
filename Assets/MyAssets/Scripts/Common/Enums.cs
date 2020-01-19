public enum ScreenState
{
    INITIALIZE,
    START,
    GAME,
    RESULT,
}

public enum GameState
{
    IN_PROGRESS_TIMER,
    RESET_TIMER,
    MOVE_UP_ANIM,
}

public enum BlockState
{
    STOP,
    FALL,
    DRAG,
}

public enum BlockType
{
    NUMBER,
    JOKER,
    ADD_NUMBER_ALL,
    FALL_LINE = 11,
    CHANGE_NUMBER_COLUMN = 12,
    ADD_TIME,

}