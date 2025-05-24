using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tamrinak_API.Migrations
{
    /// <inheritdoc />
    public partial class IconAdd2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: 8,
                column: "Base64Data",
                value: "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAACXBIWXMAAA7DAAAOwwHHb6hkAAAAGXRFWHRTb2Z0d2FyZQB3d3cuaW5rc2NhcGUub3Jnm+48GgAABHdJREFUWIWtl12IVHUYxn/vmXH9aK1QKTV152xbtjtp5pzZTTTbPqQwyiAqg4gsKqKLoCKoiy66ECwiyISKjMJE0CKFVSRKFyL62FkXrB1Lxpldk7JdyHTNNXfOebqY3XUcz8zsqg/MxXm/nue8/M/7/seogno1ewF60tAdwNVgqpwhH8gatj1L7E1sm18p2so5GtQw0eeK9WDLwN7Jk9/5u3X9UU1wq1qjvZyKg798Gmw8hrMkax3fjEtAXPGaU0xuAw5OYfDFbus+U404DLOVmDIR+w74KGepDWNOjCmxNibv4/ESukpscOX11cvbUqfFjQDXKzEjJi/jymseI3lypivv8HwtnTpeAYX81kmuvMddeb2uvBUFYcn76uXtGeNbJF+OKbH2QsiL0aBkfLSOMFeJg65uriuNi4bkNoM2XqyAjHV0A68BYAjRLqK3AL3FcU5I7kwHq3raxw/nqBFcdZ611CA0YDhTAeqVWBBT4oFLQW8ElxvOyaoCwHoFLoAPMwxbcykECPMMf39VAUZwQKgRIMpQtyB+seQNapkDzDrEvq4xCHDShpoADtn+PoPJMS268mIE+OTXGPoMIyj1nfcVBNBt2OhbC9thTHgY+LA0tjB2B54zuF1Y6NwQ9Au1Omh5mD90FLvy+iL8Mzdjmf9iStxg2G7g7pylfiuq7Lgkd4H+CtAnUXQirFae6LwIwVvC9uboeAbjnGVWTsC3EDyfs337AerlrRS8L9ho2F4fjjhohQOrspZaGVajGIVOnfwaeC9nqc+LfWFzANAB4TSNPGUttSuCnyisYr0Uga0GrwO/ViMHaLf2fADrgNWlvrBJiLC0YU3Ftox19QNvjDy7Sj4ttHAsAgAM+xM0vdQe2gFDaYa/hPIFg63AypiSj6BynSyOpxn4udQe2oEI0XRAvrFSwax1Hq9Xyz0QrHdJvot0zpQTDF7G4OKzdwk9JYIXQoSFQJiL1zcNze60zqFKQgASSkzo50xtsc1h+mCPtZ8GiCm5yNCnOUvdVJob2oHC9rJMP5HrgHQ1AcMij5XzO+hZsA/CfWUglI7gVzwHY0Fc8VrBKgg2j0uAYT8AodNrPPiXSY8Bu7LWeXxcAoaIfgHcX6/Esgsld+XNN+xVI7KuXEzZazlAnZqXOPjbheWtMGrP+08grMbQVLAaEIadFBoAJoPVGnoia507y3GEH8JhOOhGB7qEtQchtxnDaTC4zbBfhI4VBHFU6AREBozgUR9lK3FUFABaEGCbc9axqdRTp8Qsg+/BX5i1rl6AOVpyTQ35RiMCQIB5EWwRcOACBZSHg3MnqC03TD5PLU0RhnarcHiHY7hVWBrYcskFiKDWcP4+SxbMA+uBYNuILcCWGkHFc1Z1hpdPjKQM3YseigD00PEVBDvA7hr5GZwQHK5Up0oH7Dhobpgnaz+lXHndMXKbomp5JWM/HsnR+fZogLAYXptVWdkV29Ogljk+/pdg1wrt7bHUg4XLSuHKZoDQJEGNjXbTTgOnQSbYMx2trrRP/ge/vLUEsx458QAAAABJRU5ErkJggg==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: 8,
                column: "Base64Data",
                value: "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAACXBIWXMAAA7DAAAOwwHHb6hkAAAAGXRFWHRTb2Z0d2FyZQB3d3cuaW5rc2NhcGUub3Jnm+48GgAAAdpJREFUOI2Nk09rE2EQxn/z7iatGs1FPWhbk21Qa0Vpd1MpXvTgpRePikUQPFjwXFAP0pNQD4JfwoPfQOipID00EaFERNIkSPEilFbQ/OnmHQ9ldTdhxTnNvPPMM/M88AqxOKezUw5yF0zHYm28J4gj6Dqw1ZDqPoNRUn+yqMEzFBnsTepsydPy4hn1jxY1eDmt07moZ6IkxCw2Ka4i6CDBtnyoW5z3o7CkyKs2o/eHLvDUfzL0mBJxrIk3xnT+bEGDG/9LNEBgJM+P74IE/94+d94irah2o8RibY1a6BHkxnT+yI5stIs6d1XR22C7EU7RGcisRPUfxwtavmNh0+LuZThYUrQvyOWmVJKGKcYjeN6QykpCgsF+culf2pGN3aZUXrSkuiporaQzpxIEggXtDEkw7H+x5G/FsT2yr7P0HhTUP3G43FnP8XPrF9JPMaj8NM28KIoaPPTUn/i7OCFPxVM/nzZ8Qa8fFxhvSPXrkASAEdw3XQ7eFjToAwhkgXEFBf3WoxNm4F58JnFBm3AKdLkllYWWVBYEXXPYu2KQx8A7kN0QLqYSOAx+JCN16j3Q04e1HAsxI3FEQkKIfHYxjyb0WujQv6nYkwXKaxbtAh8Fu53F3YzP/AYpx6nFXD1CEwAAAABJRU5ErkJggg==");
        }
    }
}
