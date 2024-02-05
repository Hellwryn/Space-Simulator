#if defined(SHADERGRAPH_PREVIEW)
    Direstion = half3(0.5, 0.5, 0)
    color = 1
#else
    Light light = GetMainLight();
    Direction = light.direction;
    Color = light.color
#endif