#set page(
  paper: "a4",
  flipped: true,
  margin: 20pt,
)
#import "@preview/fletcher:0.5.8": diagram, edge as _edge, node as _node

#show raw: it => highlight(fill: gray.lighten(70%), it)

#let marker(fill) = box(
  rect(
    fill: fill,
    height: 1em,
    width: 1em,
  ),
  radius: .3em,
  clip: true,
  baseline: .2em,
)

#let shift = 20pt

#let edge = _edge.with(marks: "-|>", label-sep: 0pt)
#let movement(..args, label: none) = edge(..args, stroke: red, label: if label
  != none { text(fill: red, label) })
#let attack(..args, label: none) = edge(..args, stroke: orange, label: if label
  != none { text(fill: orange, label) })
#let inv(..args, label: none) = edge(..args, stroke: purple, label: if label
  != none { text(fill: purple, label) })
#let hotbar(..args, label: none) = edge(..args, stroke: fuchsia, label: if label
  != none { text(fill: fuchsia, label) })
#let event(..args, label: none) = edge(..args, stroke: green, label: if label
  != none { text(fill: green, label) })
#let node = _node.with(height: 50pt, stroke: blue)




#diagram(
  node-stroke: 1pt,
  spacing: (100pt, 50pt),
  node((0, 0), [Player Controller], name: <player>),
  node((1, 0), [Blockbreaking Manager], name: <block>),
  node((2, 0), [Tilemap Manager], name: <tilemap>),
  node((3, 0), [World Generator], name: <world>),

  node((1, 1), [Inventory], name: <inventory>),
  node((2, 1), [Resource Generator], name: <resource>),

  node((0, 2), [Hotbar], name: <hotbar>),

  movement(
    vertices: ((-1, 0), <player>),
    label: [`W` `A` `S` `D`],
    shift: shift,
  ),
  attack(vertices: ((-1, 0), <player>), label: [`left click`], shift: -shift),

  edge(<player>, <block>, label: [position], shift: shift),
  movement(<player>, <block>, label: [targettingDirection]),
  attack(<player>, <block>, label: [isHoldingBreak], shift: -shift),

  attack(<block>, <tilemap>, label: [update animation], shift: shift),
  attack(<block>, <tilemap>, label: [replace block], shift: -shift),

  edge(<world>, <tilemap>, label: [set boundary]),

  attack(<tilemap>, <resource>, label: [set resources]),

  node((1.164, 0.5), name: <helper-blockbroke>, stroke: none),
  event(
    <block.300deg>,
    <helper-blockbroke>,
    label: [on blockBroke],
    marks: "-",
  ),
  event(
    <helper-blockbroke>,
    (1.5, 0.5),
    (1.5, 1),
    <resource.west>,
    label: align(right)[start\ regeneration],
    corner: left,
  ),
  event(<helper-blockbroke>, <inventory>, label: [add loot], corner: left),

  attack(<resource>, <world.south>, label: [set resources], corner: left),

  inv((0.35, 1), <inventory>, label: [`E` (toggle inventory)], shift: -shift),

  inv(
    <inventory.west>,
    <player.south>,
    corner: right,
    label: [freeze game],
    shift: shift,
    label-pos: 30%,
  ),

  hotbar((-0.65, 2), <hotbar>, label: [`1` - `5` or `scroll`\ (change tool)]),

  node((0, 1.5), name: <helper-toolselection>, stroke: none),
  event(
    <hotbar>,
    <helper-toolselection>,
    label: [on selectionChanged],
    marks: "-",
  ),
  event(
    <helper-toolselection>,
    (0, 0.5),
    (0.832, 0.5),
    <block.240deg>,
    label: [update current\ tool],
  ),
)

// Legend
#align(right, diagram(
  node(stroke: none, height: 150pt, [
    #align(left)[= Legend:]
    #grid(
      columns: 2,
      column-gutter: 1em,
      row-gutter: 1em,
      marker(red), [Movement],
      marker(orange), [Attack/Break],
      marker(purple), [Inventory],
      marker(fuchsia), [Hotbar],
      marker(green), [Events],
    )

  ]),
))
