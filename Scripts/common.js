var _dxGridHeaderBGColor = '#F16C69';
var _dxGridHeaderColor = '#ffffff';
function dxGrid(gridName, rowCount, exportFileName, data, columns) {
    var dataGrid = $(`#+${gridName}`).dxDataGrid({
        dataSource: data,
        columnsAutoWidth: false,
        showBorders: true,
        showColumnLines: true,
        showRowLines: true,
        rowAlternationEnabled: true,
        selection: { mode: "multiple" },
        filterRow: { visible: true, applyFilter: "auto" },
        export: {
            enabled: true,
            fileName: exportFileName, allowExportSelectedData: true,
            customizeExcelCell: options => {
                if (options.gridCell.rowType === "header") {
                    options.backgroundColor = _dxGridHeaderBGColor;
                    options.font.color = _dxGridHeaderColor;
                }
            }
        },
        grouping: { autoExpandAll: true },
        groupPanel: { visible: true },
        searchPanel: { visible: true, width: 240, placeholder: "Search..." },
        onCellPrepared: function (e) {
            if (e.rowType === "header") {
                e.cellElement.css("text-align", "center");
                e.cellElement.css("font-weight", "bold");
            }
            if (e.rowType === "group") {
                e.cellElement.css("text-align", "center");
            }
        },
        headerFilter: { visible: true },
        scrolling: { columnRenderingMode: "virtual" },
        columns: columns,


        onContentReady(e) {
            jQuery(`#+${rowCount}`).html('Total Records : ' + jQuery(`#+${gridName}`).dxDataGrid('instance').totalCount());
        }

    }).dxDataGrid('instance');
}

function fileDownloadWithoutURL(fileName, FileContent) {
    if (FileContent != "") {
        var binaryString = window.atob(FileContent);
        var binaryLen = binaryString.length;
        var bytes = new Uint8Array(binaryLen);
        for (var i = 0; i < binaryLen; i++) {
            var ascii = binaryString.charCodeAt(i);
            bytes[i] = ascii;
        }
        var blob = new Blob([bytes]);
        var link = document.createElement('a');
        link.setAttribute("type", "hidden");
        fileName = fileName;
        link.download = fileName;
        link.href = window.URL.createObjectURL(blob);
        document.body.appendChild(link);
        link.click();
        link.remove();
    }
    else {
    }

}
