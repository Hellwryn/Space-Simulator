using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Represents a single type of working pop of a single specie working in a Region
Can be of a multiple cultures, but of a single specie, and living in a single type of environment
*/

public class Pop
{
    int count;
    int scale;
    int specie;

    // Cast
    int AffiliatedFaction; // Which faction the pop belongs to, usually the state, sometime a megacorp/church/etc...
    CastLib castLib;
    CastActivity castActivity;
    CastResidency castResidency;
    bool Noble;
    bool Military;
    bool StateWorker;
    bool Intellectual;

    // Ideologies
    float ColInd; // Collectivism-Individualism
    float ColIndRad;
    float IdeMat; // Idealism-Materialism
    float IdeMatRad;
    float TraInn; // Traditionnalism-Isolation
    float TraInnRad;
    float DivSeg; // Diversity-Segragation
    float DivSegRad;
}

public enum CastLib
{
    Slave,
    Resident,
    Citizen
}

public enum CastActivity
{
    NonActive,
    Worker,
    Clerk,
    Independant,
    Owner,
    Mogul
}

public enum CastResidency
{
    Extern,
    Confined,
    Rural,
    Urban,
    Dense
}