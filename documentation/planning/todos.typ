#set enum(spacing: 1.5em)
#set list(marker: math.square.stroked.big)
#set strike(stroke: 1.5pt)

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

#let optional = body => {
  set text(fill: gray)
  [[OPTIONAL]: ]
  body
}

#let colorize(col) = it => text(fill: col, weight: "bold", it)
#show "Leon": colorize(blue)
#show "Tim": colorize(lime.darken(50%))
#show "Sinan": colorize(yellow.darken(50%))
#show "Nils": colorize(purple)
#show "everyone": colorize(orange)
#show "todo": colorize(red)

= Next Steps
#context {
  text(
    fill: black.lighten(40%),
    size: text.size * 80%,
  )[TODOs unitl Friday, 20.03.]
}
- write blog article Leon
// - adapt walk-animation speed to walk speed todo
- #strike[refactor Tim]
- #strike[treasures hidden in Lehm blocks Leon]
- #strike[small icon on how to interact with the targeted block Nils]
- levels: Leon + Sinan
  - #strike[write world creation logic for forest, mine, ...]
  - add second layer
  - design starter level: starter house with crafting table, starter treasure; some resources on the map; portals to the other worlds
  - #strike[level/world switching logic, door/portal etc.]
  - limited vision
- #strike[think of storyline and progression system #math.arrow create a plan (figma/excalidraw) Leon + Sinan]
- #strike[add mobs (that only run around for now) Nils]
- #optional[tool animations Sinan]
- #optional[modular 3D tool presets (change blade based on material)]

#pagebreak(weak: true)
= Next Steps
#context {
  text(
    fill: black.lighten(40%),
    size: text.size * 80%,
  )[TODOs unitl Friday, 13.03.]
}
- #strike[write blog article Leon]
- #strike[add hotbar Nils]
- #strike[add tools Tim]
- add tool animation (as alternative to attack) Sinan
- #strike[add tool logic (block has _tool_ and _hardness_ attribute #math.arrow calculate mineability and speed) Tim]
- #strike[add `crafting table` and `anvil` as interactable blocks with crafting logic:\ crafting recipes as JSON or similar Tim]
- #strike[#optional[add mobs (that only run around for now)]]
- #strike[add loot-tables for blocks Leon]
- #strike[add treasures Leon]
- #strike[add more materials Leon]
- #strike[add achievements (only the screen and a json/... template for achievements) Nils]
- #strike[add achievements logic Nils]
- #strike[#optional[implement level creation-logic Sinan]]
- #optional[Add background music]
- #optional[Add sound effects for block break etc.]

#pagebreak(weak: true)
= Backlog
- #optional[Add background music]
- #optional[Add sound effects for block break etc.]

== Game Idea
- spawn in house/base, entry world has portals to further worlds
- one world for every resource

== Feedback
- "villagers"/farm workers can be bought and placed
- weather conditions like fog where you cant see the whole map so that would make it a bit harder to find stuff
  - torch for more viewing distance
  - craft sth like blitz in pokemon where you can then see the whole map?
- #strike[achievements]
- upgradable tool #math.arrow enchantments
  - man könnte auch so 2x2 enchantments hinzufügen (also dass man statt 1x1 2x2 abbauen kann)
- mobs disturbing you from mining
- #optional[food]
- armor for the player to protect against mobs
- #optional[day-night cycle]


#pagebreak(weak: true)
= Minimum Viable Product
#context {
  text(
    fill: black.lighten(40%),
    size: text.size * 80%,
  )[TODOs for Wednesday / until Friday]
}
#v(1em)
- #strike[cleanup _testing-and-playing-around_-files and push on GitHub Tim]
- #strike[inventory listens on block-break-event and puts item into inventory Nils]
- player animation with tool in hand (depending on selected tool) Sinan
- #strike[merge everything everyone]
- #strike[block-break animation Tim]
- #strike[logic: what happens in what event (documentation): Leon
    #list(
      marker: math.arrow,
      [e.g. breaking iron spawns cobble, cobble may convert to iron after 5s (probability: 20%)],
      [types of materials we use: iron, cobblestone, wood, diamond, water, bricks],
      [clean stone cannot be passed, wood can be broken #math.triangle.r.filled becomes passable],
    )]
- #strike[Design map with a bit of a story (maybe a little labyrinth with some different blocks) Leon]
- #strike[#optional[Design hotbar Nils]]
- #strike[#optional[blocks can only be broken with special tools #math.triangle.r.filled pickaxe, ...
    #list(
      marker: math.arrow,
      [player has one tool selected at a time],
      [every block has a hardness and tool group (pickaxe, shovel, or axe)],
      [every tool has a hardness (only block with lower or equal hardness can be broken)],
      [blocks don't break if used with tool that is not hard enough],
    )]]
- #strike[#optional[draw architecture diagram]]
