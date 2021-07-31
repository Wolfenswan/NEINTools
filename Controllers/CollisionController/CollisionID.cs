// CollisionID-enum is used to maintain accuracy in all related methods when checking for collision states
// They are also used as keys in the dictionary the CollisionController returns on UpdateCollisions()

public enum CollisionID 
{
    GROUND,
    SPAWN_OR_EXIT,
    CEILING,
    ENTITY_LEFT,
    ENTITY_RIGHT,
    PLAYER_LEFT,
    PLAYER_RIGHT,
    PLAYER_DOWN,
    ENTITY_ABOVE,
    POW,
    PLAYER_ABOVE
}