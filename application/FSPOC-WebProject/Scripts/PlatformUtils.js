function CurrentModuleIs(moduleClass) {
    return $("body." + moduleClass).length ? true : false;
}
