#set page(paper: "a5", flipped: true)
#let date = it => {
  pagebreak(weak: true)
  align(center, text(size: 2em, weight: "bold", it))
}
#let action = it => text(fill: gray, it)
#let crafting(..elements) = grid(columns: (
    auto,
    auto,
    auto,
  ), column-gutter: 1em, row-gutter: 0.5em, ..elements)
#set heading(numbering: "1.1")

#date[27.03.2026: Analytics]
ForgeForward2D: The game for endless mining, collecting, and exploring

= Demo
- #action[show game]
- mobs chasing player and stealing items
- anvil
- achievements
- new portals
- new levels
- (NPC interaction)

= Analytics
- #action[show analytics and explain a bit]

#date[20.03.2026: Addictive Patterns]
- ForgeForward2D: The game for endless mining, collecting, and exploring

= Demo
- #action[show game]
- treasures
- rare blocks in clay
- loot tables
- crafting recipes
- interaction indicator
- automatic world creation
- MOBS

#pagebreak(weak: true)
= Big refactor
- #action[show architecture diagram and explain]

#pagebreak()

#date[13.03.2026: Unity & Software Architecture]
- ForgeForward2D: The game for endless mining, collecting, and exploring

= Demo
- #action[show game]
- treasures
- loot tables
- #action[explain new animation]
- next week: 3D tools
- hardness and tool on every block, some blocks require special tool level
- ??? crafting ??? // TODO
- #action[show inventory]

#pagebreak(weak: true)
= Architecture
- #action[show architecture diagram and explain]

== Biggest Problems
- Merge conflicts in `Scene.unity` file

#pagebreak()

#date[06.03.2026: MVP]
- ForgeForward2D: The game for endless mining, collecting, and exploring
= Demo
- #action[show game]
- run around, discover a world full of treasures, wood, and saplings
- collect wood
- collect iron ore
- collect stone
- collect diamonds
- ore and wood regenerate, stone not
- #action[show inventory]
- #strong[Goal of the game: collecting ores, discovering the world, upgrading your stuff]

#pagebreak(weak: true)
= Next Steps
- introduce tooling model:
  - player has a hotbar to be filled with tools
  - without tool, player can only mine `wood`
  - introduce `crafting table`: craft together different materials:
    #crafting(
      [`wood` + `wood`],
      [$=>$],
      [`wooden hammer` (breaks `stone`)],
      [`wood` + `stone`],
      [$=>$],
      [`stone hammer` (breaks `stone` faster)],
      [`wood` + `iron ingot`],
      [$=>$],
      [`iron hammer`],
      [`iron ingot` + `iron ingot`],
      [$=>$],
      [`iron pickaxe` (breaks `iron ore` and `diamond ore`)],
      [`iron ingot` + `diamond`],
      [$=>$],
      [`diamond pickaxe`],
    )
  - introduce `anvil` (can be found on map):
    - when used with `iron hammer`:
      #crafting(
        [`iron ore`],
        [$=>$],
        [`iron ingot`],
        [`diamond ore`],
        [$=>$],
        [`diamond`],
      )
  - add mining animation
- introduce hostile mobs:
  - no HP-system (player cannot die), but little zombies steal resources from inventory if player does not kill them
- introduce treasures:
  - hidden on map
  - player has to find his first `iron ingot`
- introduce more materials, design cooler levels

