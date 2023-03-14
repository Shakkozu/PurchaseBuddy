import { FlatTreeControl } from '@angular/cdk/tree';
import { Component } from '@angular/core';
import { MatTreeFlatDataSource, MatTreeFlattener } from '@angular/material/tree';

/**
 * Food data with nested structure.
 * Each node has a name and an optional list of children.
 */
interface FoodNode {
  name: string;
  children?: FoodNode[];
}

interface ProductNode {
  name: string;
  guid: string;
  children?: ProductNode[];
}

const products_data = [
    {
      "guid": "9d8b646f-16e3-4dec-a5df-77e55dd970ba",
      "name": "Food products",
      "children": [],
    },
    {
      "guid": "82347536-c93e-4dcf-bf82-0ed1e508b7e9",
      "name": "Beverages",
      "children": [
        {
          "guid": "a20a6a45-2581-4604-97b3-14b2e896036a",
          "name": "Beer",
          "children": [],
        }
      ],
    },
    {
      "guid": "0de1c1c4-211f-4cf1-846d-897a40a89743",
      "name": "Bakery products",
      "children": [
        {
          "guid": "447dc85c-6d5b-420b-a81e-c7581a68a97f",
          "name": "Bread",
          "children": [],
        }
      ],
    }
  ]

const TREE_DATA: FoodNode[] = [
  {
    name: 'Fruit',
    children: [{ name: 'Apple' }, { name: 'Banana' }, { name: 'Fruit loops' }],
  },
  {
    name: 'Vegetables',
    children: [
      {
        name: 'Green',
        children: [{ name: 'Broccoli' }, { name: 'Brussels sprouts' }],
      },
      {
        name: 'Orange',
        children: [{ name: 'Pumpkins' }, { name: 'Carrots' }],
      },
    ],
  },
];

/** Flat node with expandable and level information */
interface ExampleFlatNode {
  expandable: boolean;
  name: string;
  level: number;
}

/**
 * @title Tree with flat nodes
 */
@Component({
  selector: 'tree-flat-overview-example',
  templateUrl: 'product-details.component.html',
})
export class UserProductDetailsComponent {
  private _transformer = (node: ProductNode, level: number) => {
    return {
      expandable: !!node.children && node.children.length > 0,
      name: node.name,
      level: level,
    };
  };

  treeControl = new FlatTreeControl<ExampleFlatNode>(
    node => node.level,
    node => node.expandable,
  );

  treeFlattener = new MatTreeFlattener(
    this._transformer,
    node => node.level,
    node => node.expandable,
    node => node.children,
  );

  dataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);

  constructor () {
    this.dataSource.data = products_data;
    // this.dataSource.data = TREE_DATA;
  }

  hasChild = (_: number, node: ExampleFlatNode) => node.expandable;
}
