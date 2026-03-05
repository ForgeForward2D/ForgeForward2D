#set enum(spacing: 1.5em)
#set list(marker: math.square.stroked.big)

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

= Minimum Viable Product
#context {
  text(
    fill: black.lighten(40%),
    size: text.size * 80%,
  )[TODOs for Wednesday / until Friday]
}
#v(1em)
- #strike[cleanup _testing-and-playing-around_-files and push on GitHub Tim]
- inventory listens on block-break-event and puts item into inventory Nils
- player animation with tool in hand (depending on selected tool) Sinan
- merge everything everyone
- block-break animation Tim
- logic: what happens in what event (documentation): Leon
  #list(
    marker: math.arrow,
    [e.g. breaking iron spawns cobble, cobble may convert to iron after 5s (probability: 20%)],
    [types of materials we use: iron, cobblestone, wood, diamond, water, bricks],
    [clean stone cannot be passed, wood can be broken #math.triangle.r.filled becomes passable],
  )
- Design map with a bit of a story (maybe a little labyrinth with some different blocks) Leon
- #optional[Design hotbar Nils]
- #optional[blocks can only be broken with special tools #math.triangle.r.filled pickaxe, ...
    #list(
      marker: math.arrow,
      [player has one tool selected at a time],
      [every block has a hardness and tool group (pickaxe, shovel, or axe)],
      [every tool has a hardness (only block with lower or equal hardness can be broken)],
      [blocks don't break if used with tool that is not hard enough],
    )]
- #optional[draw architecture diagram]

#pagebreak(weak: true)
= Ideas
- Separate Item and Block abstraction?
  - Collect attributes and create sciptable objects for them
    - Block
      - ID
      - Tile Reference
      - spawn rate
      - resource item id
      - resource replacement id
      - walkable?
      - breakable/farmable?
    - Item
      - ID
      - Max stack
      - Display name
      - Sprite


// ## Todos
// ### World
// - Map design
// 	- support for different backgrounds?
// - Decide on a resource generation and breaking method
// 	- current: ore respawns in fixed location and turns into stone / sapling temporary
// 	- how to handle stone as a resource?
// 	- let the player destroy stuff?
// - Handle resource generation, when the player stands on sapling
// - include metadata array

// ### Player
// - Steve sprite with animation
// - tools and animation
// - Solves glitches with Tilemap and moving camera

// ### Integration
// - Separate Item and Block abstraction?
// 	- Collect attributes and create sciptable objects for them
// 		- Block
// 			- ID
// 			- Tile Reference
// 			- spawn rate
// 			- resource item id
// 			- resource replacement id
// 			- walkable?
// 			- breakable/farmable?
// 		- Item
// 			- ID
// 			- Max stack
// 			- Display name
// 			- Sprite
// - Subscribe inventory to block breaking event
// - setup git and join code
// 	- think about architecture / folder hierarchy
// - draw architecture diagram?

// ### Inventory / Crafting / Interactables?
