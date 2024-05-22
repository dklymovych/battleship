document.addEventListener("DOMContentLoaded", function() {
    const grid = document.getElementById("grid");
    const shipsContainer = document.querySelector('.ships-container');
    const ships = document.querySelectorAll(".ship");
    const output = document.getElementById("output");
    const toggleModeButton = document.getElementById("toggleMode");
    const clearGridButton = document.getElementById("clearGrid");
    const randomizeGridButton = document.getElementById("randomizeGrid");
    let placementMode = "horizontal"; // Initial mode
    let shipPosition = {
        "4": [],
        "3": [],
        "2": [],
        "1": []
    };
    const gridSize = 10;
    let draggedShip = null;

    // Create grid cells
    for (let i = 0; i < gridSize * gridSize; i++) {
        let cell = document.createElement("div");
        cell.dataset.x = i % gridSize;
        cell.dataset.y = Math.floor(i / gridSize);
        cell.addEventListener("dragover", allowDrop);
        cell.addEventListener("drop", drop);
        grid.appendChild(cell);
    }

    ships.forEach(ship => {
        ship.addEventListener("dragstart", dragStart);
        ship.addEventListener("dragend", dragEnd);
    });

    function dragStart(event) {
        const length = event.target.dataset.length;
        const elementId = event.target.id;
        event.dataTransfer.setData("length", length);
        event.dataTransfer.setData("elementId", elementId);
        event.dataTransfer.setData("mode", placementMode);

        // Hide the ship from the list while dragging
        event.target.style.opacity = "0";
        draggedShip = event.target;

        // Create a custom drag image
        const dragImage = document.createElement('div');
        dragImage.className = 'ship';
        dragImage.style.position = 'absolute';
        dragImage.style.pointerEvents = 'none';
        dragImage.style.zIndex = '1000';

        if (placementMode === "vertical") {
            dragImage.style.width = "40px";
            dragImage.style.height = `${40 * length}px`;
        } else {
            dragImage.style.width = `${40 * length}px`;
            dragImage.style.height = "40px";
        }

        dragImage.style.backgroundColor = 'rgba(128, 128, 128, 0.5)';
        document.body.appendChild(dragImage);
        event.dataTransfer.setDragImage(dragImage, 20, 20);

        // Remove the custom drag image after a short delay to ensure it doesn't remain in the DOM
        setTimeout(() => {
            document.body.removeChild(dragImage);
        }, 0);
    }

    function dragEnd(event) {
        // Show the ship back in the list by restoring its opacity
        if (draggedShip) {
            draggedShip.style.opacity = "1";
            draggedShip = null;
        }
    }

    function allowDrop(event) {
        event.preventDefault();
    }

    function drop(event) {
        event.preventDefault();
        let length = parseInt(event.dataTransfer.getData("length"));
        let elementId = event.dataTransfer.getData("elementId");
        let mode = event.dataTransfer.getData("mode");
        let x = parseInt(event.target.dataset.x);
        let y = parseInt(event.target.dataset.y);

        if (canPlaceShip(x, y, length, mode)) {
            let cells = [];
            for (let i = 0; i < length; i++) {
                let cell;
                if (mode === "horizontal") {
                    cell = document.querySelector(`div[data-x="${x + i}"][data-y="${y}"]`);
                } else {
                    cell = document.querySelector(`div[data-x="${x}"][data-y="${y + i}"]`);
                }
                if (cell) {
                    cell.style.backgroundColor = "darkgray";
                    cells.push({ x: parseInt(cell.dataset.x), y: parseInt(cell.dataset.y) });
                }
            }
            shipPosition[length].push(cells);
            document.getElementById(elementId).remove();
            updateRowVisibility(); // Update row visibility
        } else {
            alert("Cannot place ship here. Ensure there is enough space around the ship.");
            if (draggedShip) {
                draggedShip.style.opacity = "1"; // Revert opacity if placement failed
                draggedShip = null;
            }
        }
    }

    function canPlaceShip(x, y, length, mode) {
        for (let i = 0; i < length; i++) {
            // Check boundaries
            if (mode === "horizontal" && (x + i >= gridSize)) return false;
            if (mode === "vertical" && (y + i >= gridSize)) return false;

            for (let dx = -1; dx <= 1; dx++) {
                for (let dy = -1; dy <= 1; dy++) {
                    let nx = (mode === "horizontal") ? x + i + dx : x + dx;
                    let ny = (mode === "horizontal") ? y + dy : y + i + dy;
                    let cell = document.querySelector(`div[data-x="${nx}"][data-y="${ny}"]`);
                    if (cell && cell.style.backgroundColor === "darkgray") {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    function updateRowVisibility() {
        const rows = document.querySelectorAll('.ships-row');
        rows.forEach(row => {
            const shipsInRow = row.querySelectorAll('.ship');
            let allHidden = true;
            shipsInRow.forEach(ship => {
                if (ship.style.display !== 'none') {
                    allHidden = false;
                }
            });
            row.style.display = allHidden ? 'none' : 'flex';
        });
    }

    function clearGrid() {
        // Clear the grid
        const gridCells = grid.querySelectorAll('div');
        gridCells.forEach(cell => {
            cell.style.backgroundColor = "lightgray";
        });

        // Clear the ship positions
        shipPosition = {
            "4": [],
            "3": [],
            "2": [],
            "1": []
        };

        // Restore ships to the original positions
        const shipRows = {
            "4": document.getElementById("row4"),
            "3": document.getElementById("row3"),
            "2": document.getElementById("row2"),
            "1": document.getElementById("row1")
        };

        ships.forEach(ship => {
            const length = ship.dataset.length;
            shipRows[length].appendChild(ship);
            ship.style.display = 'flex';
            ship.style.opacity = '1';
            ship.style.transform = ''; // Reset transform
        });

        updateRowVisibility(); // Update row visibility after clearing grid
    }

    function getRandomInt(max) {
        return Math.floor(Math.random() * max);
    }

    function placeShipRandomly(length) {
        let placed = false;
        while (!placed) {
            const x = getRandomInt(gridSize);
            const y = getRandomInt(gridSize);
            const mode = getRandomInt(2) === 0 ? "horizontal" : "vertical";
            if (canPlaceShip(x, y, length, mode)) {
                let cells = [];
                for (let i = 0; i < length; i++) {
                    let cell;
                    if (mode === "horizontal") {
                        cell = document.querySelector(`div[data-x="${x + i}"][data-y="${y}"]`);
                    } else {
                        cell = document.querySelector(`div[data-x="${x}"][data-y="${y + i}"]`);
                    }
                    if (cell) {
                        cell.style.backgroundColor = "darkgray";
                        cells.push({ x: parseInt(cell.dataset.x), y: parseInt(cell.dataset.y) });
                    }
                }
                shipPosition[length].push(cells);
                placed = true;
            }
        }
    }

    randomizeGridButton.addEventListener("click", function() {
        clearGrid();

        const shipLengths = [4, 3, 3, 2, 2, 2, 1, 1, 1, 1];
        shipLengths.forEach(length => {
            placeShipRandomly(length);
        });

        ships.forEach(ship => {
            const length = ship.dataset.length;
            if (shipPosition[length].length > 0) {
                ship.style.display = 'none';
            }
        });

        updateRowVisibility(); // Update row visibility after randomizing grid
    });

    clearGridButton.addEventListener("click", clearGrid);

    toggleModeButton.addEventListener("click", function() {
        if (placementMode === "horizontal") {
            placementMode = "vertical";
            toggleModeButton.textContent = "Vertical";
        } else {
            placementMode = "horizontal";
            toggleModeButton.textContent = "Horizontal";
        }
    });

    function getOrderedShipPosition() {
        return ["4", "3", "2", "1"].reduce((obj, key) => {
            obj[key] = shipPosition[key];
            return obj;
        }, {});
    }

    document.getElementById("joinRoomBtn").addEventListener("click", function(event) {
        event.preventDefault();
        const orderedShipPosition = getOrderedShipPosition();

        var shipJSON = JSON.stringify(orderedShipPosition);
        console.log(shipJSON);
    
        document.getElementById("shipPositions").value = shipJSON;
        
        event.target.form.submit();
    });

    document.getElementById("joinRandomRoomBtn").addEventListener("click", function(event) {
        event.preventDefault();
        const orderedShipPosition = getOrderedShipPosition();

        var shipJSON = JSON.stringify(orderedShipPosition);
        console.log(shipJSON);

        document.getElementById("shipPositionsRandom").value = shipJSON;

        event.target.form.submit();
    });
    
});
